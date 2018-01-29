using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class ConstructionEdit : MonoBehaviour
{
    public static ConstructionEdit instance;

    List<string> Segments = new List<string>
    {
        "Girder", "GirderFixed", "Wheel", "Suspension"
    };
    public int CurSegment = 0;

    public GameObject clamping;
    public GameObject suspension;
    public GameObject joint;
    public GameObject wheel;

    public Transform Container;
    bool editmode = false;

    void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        EnablePhysics(false);
        ChoiceNextSegment();
    }

    void ChoiceNextSegment() {
        CurSegment ++;
        if (CurSegment > Segments.Count - 1)
        {
            CurSegment = 0;
        }
        UIManager.instance.SelectedSegmentChange(Segments[CurSegment]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChoiceNextSegment();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (editmode || UI_Utils.IsPointerOverUIObject) return;
            GameObject selected = GetJoint();

            switch (CurSegment)
            {
                case 0:
                    StartCoroutine(CreateGirder(selected));
                    break;
                case 1:
                    StartCoroutine(CreateGirder(selected, true));
                    break;
                case 2:
                    if (selected != null)
                        CreateWheel(selected);
                    break;
                case 3:
                    StartCoroutine(CreateGirder(selected, false, true));
                    break;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            EnablePhysics(!PhysEnabled);
        }

    }


    IEnumerator CreateGirder(GameObject start, bool Fixed = false, bool spring = false) {
        editmode = true;
        yield return new WaitForSeconds(0.01f);
        Vector2 startPoint = WorldMouse();


        GameObject firstGO = null;

        if (start != null){
            startPoint = start.transform.position;
            firstGO = start;
        }else{
            firstGO = MakeJoint(joint, startPoint);
            firstGO.transform.position = startPoint;
        }

        GameObject clampingGO = null;
        if(spring)
            clampingGO = Instantiate(suspension, Container);
        else
            clampingGO = Instantiate(clamping, Container);


        GameObject secondGO = MakeJoint(joint, startPoint);
        secondGO.GetComponent<CircleCollider2D>().enabled = false;

        while (!Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2 targ = WorldMouse();
            UpdateClamping(clampingGO.transform, startPoint, targ);
            secondGO.transform.position = targ;

            //abort create
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
               if(start==null) Destroy(firstGO);
                Destroy(clampingGO);
                Destroy(secondGO);
                editmode = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        //проверяем где отпустили
        
        GameObject selected = GetJoint();
        //если под курсором ось значит клеим к ней
        if (selected != null)
        {
            Destroy(secondGO);
            secondGO = selected;
            Vector2 targ = selected.transform.position;
            UpdateClamping(clampingGO.transform, startPoint, targ);
        }
        else {
            secondGO.GetComponent<CircleCollider2D>().enabled = true;
        }

        Rigidbody2D clampingRB = clampingGO.GetComponent<Rigidbody2D>();

        //проверка на повторное соединение
        HingeJoint2D[] joints = secondGO.GetComponents<HingeJoint2D>();
        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i].connectedBody == clampingRB) {
                //abort create
                if (start == null) Destroy(firstGO);
                Destroy(clampingGO);
                Destroy(secondGO);
                editmode = false;
                yield break;
            }
        }



        //подключаем физику
        if (spring) {
            SpringJoint2D joint1_ = firstGO.AddComponent<SpringJoint2D>();
            joint1_.connectedBody = secondGO.GetComponent<Rigidbody2D>();
            joint1_.dampingRatio = 1;
            joint1_.frequency = 2;
            joint1_.autoConfigureDistance = false;
            Suspension sp = clampingGO.AddComponent<Suspension>();
            sp.Init(firstGO, secondGO);
        }
        else
        {
            HingeJoint2D joint1_ = firstGO.AddComponent<HingeJoint2D>();
            HingeJoint2D joint2_ = secondGO.AddComponent<HingeJoint2D>();
            //connect
            joint1_.connectedBody = clampingRB;
            joint2_.connectedBody = clampingRB;
            //limit
            if (Fixed)
            {
                joint1_.useLimits = joint2_.useLimits = true;
                joint1_.limits = joint2_.limits = new JointAngleLimits2D { max = 0 };
            }
        }
        //
        OnCompleteCreate();
        editmode = false;
    }


    void UpdateClamping(Transform clamping, Vector2 a, Vector2 b) {
        Vector2 dir = b - a;
        Vector2 pos = a + dir / 2;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        clamping.eulerAngles = new Vector3(0, 0, angle);
        clamping.position = pos;
        clamping.localScale = new Vector3(dir.magnitude, 1, 1);
    }
    void OnCompleteCreate() {

    }


    void CreateWheel(GameObject axis) {
        GameObject wheelGo = Instantiate(wheel, Container);
        wheelGo.transform.position = axis.transform.position;
        HingeJoint2D joint1_ = wheelGo.AddComponent<HingeJoint2D>();
        joint1_.connectedBody = axis.GetComponent<Rigidbody2D>();
        wheelGo.AddComponent<Wheel>();
    }

    //extensions
    public static bool PhysEnabled = true;
    public void EnablePhysics(bool status) {
        foreach (Rigidbody2D item in GameObject.FindObjectsOfType(typeof(Rigidbody2D)))
        {
            if(status == true)
                item.constraints = RigidbodyConstraints2D.None;
            else
                item.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        PhysEnabled = status;
        UIManager.instance.PlayStatusChange();
    }
    GameObject GetJoint() {
        Vector3 mouse = WorldMouse();

        Collider2D[] hit = Physics2D.OverlapPointAll(mouse);
        if (hit.Length>0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].name.Contains("joint"))
                {
                    return hit[i].gameObject;
                }
            }
        }
        return null;
    }
    Vector3 WorldMouse() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    GameObject MakeJoint(GameObject prefab, Vector2 pos) {
        GameObject go = Instantiate(prefab, Container);
        go.name = "joint";
        go.transform.position = pos;
        return go;
    }
}
