using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxManager : MonoBehaviour
{
    public static DialogBoxManager instance;

    public Text TalkText;//�Ի��ı���
    private AudioSource printAudio;//������Ч
    public GameObject Bg; //����ͼƬ
    public GameObject LeftPerson, CenterPerson, RightPerson;//�����������ֵ�λ��
    public Dictionary<string, GameObject> PositionToPersonPicture;//�ֵ䣬ͨ����������ȡ��Ӧ������ͼƬ���壬��"��"��ӦcenterPerson�������
    public GameObject TalkBg;//�Ի���������
    public GameObject LeftName;
    public GameObject Logs, LogParent, LogInstance, LogScrollBar;
    public bool showTalkTexting = false;
    public GameObject BgmVolume, VoiceVolume, TextSpeed;//�ֶ���
    public GameObject SettingsPanel;

    //Э�̵Ĺ�������������stopCorution�޷�ֹͣ��������Э�̣������ڻ�ò���������Ϊ������������Э����ʹ����Щ��������
    //Ҳ�������������������ǱȽϼ򵥵�һ��
    public string text;
    public float showTextSpeed = 8;//ÿ����ʾ���ֵĸ���
    private string newBgPath;
    private float durationTime;

    private void Awake()
    {
        instance = this;
        LeftName = GameObject.Find("LeftName");
        Bg = GameObject.Find("SceneBg");
        TalkBg = GameObject.Find("TalkBg");
        printAudio = GetComponent<AudioSource>();
        TalkText = GameObject.Find("TalkText").GetComponent<Text>();
        LeftPerson = GameObject.Find("LeftPerson");
        CenterPerson = GameObject.Find("CenterPerson");
        RightPerson = GameObject.Find("RightPerson");
        PositionToPersonPicture = new Dictionary<string, GameObject> { { "��", LeftPerson }, { "��", CenterPerson }, { "��", RightPerson } };
    }

    void Start()
    {}


    public void SetRaycastTargetOn(Transform t)//�������弰�����������壨�������в㼶������������壩��image�����raycastTargetΪon
    {

        foreach (Transform child in t.GetComponentsInChildren<Transform>())
        {
            //Debug.Log("������" + child.name + "��RaycastTarget");
            if (child.GetComponent<Image>())
                child.GetComponent<Image>().raycastTarget = true;
        }
    }
    public void SetRaycastTargetOff(Transform t)
    {

        foreach (Transform child in t.GetComponentsInChildren<Transform>())
        {
            //Debug.Log("�ر���" + child.name + "��RaycastTarget");
            if (child.GetComponent<Image>())
                child.GetComponent<Image>().raycastTarget = false;
        }
    }


    public void LoadPicture(string path, string position)//���ݾ籾��������Ӧλ����ʾ����
    {

        Sprite tempsprite = (Sprite)Resources.Load(path, typeof(Sprite));

        Debug.Log("����:����ͼƬ---����·��:" + path + ",λ�ã�" + PositionToPersonPicture[position]);
        GameObject PersonPicture = PositionToPersonPicture[position];

        PersonPicture.GetComponent<Image>().sprite = tempsprite;

        PersonPicture.GetComponent<Image>().color = new Color(1, 1, 1, 1);


    }
    //��������
    public void SwitchPerson(string position, string type)
    {
        if (position == "All")
        {
            if (type == "on")
            {
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 1);

            }
            else
            {
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PositionToPersonPicture["��"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            return;
        }
        if (type == "on")
        {
            PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
        else
        {
            PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }//������Ӧλ�õ����棬������ͨ������͸����ֵ�����ػ���



    public void AddPerson(string name, string position, string picturePath)
    {

        Sprite tempsprite = (Sprite)Resources.Load("Pictures/Person/" + name + "/" + picturePath, typeof(Sprite));
        PositionToPersonPicture[position].GetComponent<Image>().sprite = tempsprite;
        PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 1);

    }//��ĳ��������������������ؾ籾��Ҳ�����������

    //�л�����ͼ
    public void TurnBackground(string newBgPath, float durationTime)
    {
        StopCoroutine("TurnBgCoroutine");       //�ر���һ���л�������Э��
        this.newBgPath = newBgPath;
        this.durationTime = durationTime;
        StartCoroutine("TurnBgCoroutine");//��Э�̼�ʱ�������л�����ͼƬ
    }//���ı���ͼƬ

    public void ShowTalkText(string text, float speed)//һ��һ������ʾ�Ի��ı�
    {
        StopCoroutine("ShowTalkTextCoroutine");

        this.text = text;
        this.showTextSpeed = speed;

        StartCoroutine("ShowTalkTextCoroutine");
    }
    IEnumerator ShowTalkTextCoroutine()
    {
        showTalkTexting = true;
        string tempText = "";
        for (int i = 0; i < text.Length; i++)
        {

            tempText += text[i];
            //Debug.Log(tempText);
            printAudio.Play();
            TalkText.text = tempText.Replace("\\n", "\n");//�����ı��л��з�
            yield return new WaitForSeconds(1 / showTextSpeed);

        }
        showTalkTexting = false;
    }
    IEnumerator TurnBgCoroutine()
    {
        Sprite newBg = (Sprite)Resources.Load("Pictures/Bg/" + newBgPath, typeof(Sprite));

        Bg.GetComponent<Image>().CrossFadeAlpha(0.4f, durationTime, true);
        yield return new WaitForSeconds(durationTime);
        Bg.GetComponent<Image>().sprite = newBg;

        Bg.GetComponent<Image>().CrossFadeAlpha(1, durationTime, true);
    }
    //IEnumerator ShowPictureSlowly()
    //{
    //    PositionToPersonPicture[LoadPicturePosition].GetComponent<Image>().CrossFadeAlpha(1, 1, true);
    //    yield return new WaitForSeconds(1);
    //}

    public void SwitchWindow(string type)
    {
        if (type == "on")
        {
            TalkBg.SetActive(true);
            LeftName.SetActive(true);

        }
        else
        {
            TalkBg.SetActive(false);
            LeftName.SetActive(false);
        }
    }//���ضԻ���


    public void OpenLogs()
    {
        Logs.SetActive(true);

        //���Logs������������
        for (int i = 0; i < LogParent.transform.childCount; i++)
        {
            Destroy(LogParent.transform.GetChild(i).gameObject);
        }
        int LogNum = 0;
        int startLine = 0;//��ʼ��ӡ����
        int index = TextParser.instance.index;
        string[] Treatment = TextParser.instance.Treatment;
        for (int i = index; i >= 0; i--)
        {
            if (Treatment[i][0] != '@' && Treatment[i][0] != '#')//���������к�ע����
            {

                LogNum++;
                if (LogNum == 30)
                {
                    startLine = i;
                    break;
                }

            }
            else
            {
                continue;
            }
        }
        DialogBoxManager.instance.SwitchWindow("off");
        for (int i = startLine; i < index; i++)
        {
            if (Treatment[i][0] == '@' || Treatment[i][0] == '#')
            {
                continue;
            }

            GameObject tempInstance = Instantiate(LogInstance, new Vector3(0, 0, 0), transform.rotation, LogParent.transform);
            string[] treatmentText = Treatment[i].Split('|');
            string tempName = treatmentText[0];
            string tempText = treatmentText[1];
            tempInstance.transform.Find("LogBg").transform.Find("Text").GetComponent<Text>().text = "��" + tempName + "��\t[" + tempText + "]";
        }
        LogScrollBar.GetComponent<Scrollbar>().value = 0.01f;
    }//�����ı��ط����
    public void CloseLogs()
    {
        Logs.SetActive(false);
        DialogBoxManager.instance.SwitchWindow("on");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//����esc��ť�����������
        {

            SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);
        }
    }
}
