using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextParser : MonoBehaviour
{
    public static TextParser instance;

    private Text LeftName;
    public bool AutoPlaying = false;//�Ƿ���ִ���Զ�����Э��
    private AudioSource voiceSource;

    public string[] Treatment;

    public GameObject LogScrollBar;
    public int startIndex = 0;//������
    public int index = 0;
    public string[] info;
    public float TextSpeed;
    public string protagonist = "����";//���˹�����
    public float turnBgTime = 1.5f;
    public GameObject Logs, LogInstance, LogParent;
    public bool End = false;
    public float waitAudioTime = 1;//�Զ�����ʱ����
    public bool treatmentAutoPlay = false;//�Զ�����
    public float playTime = -1;//��ǰ����ʱ��
    public string treatmentName = "Test";//��txt��׺
    public string currentTreatmentText, currentPersonName, currentPersonPicturePath;
    public string oldPersonPicture;//�������޷��������ͼƬ·��ʱ�ľɵ�����ͼƬ·��


    private void Awake()
    {
        instance = this;

        LeftName = GameObject.Find("LeftName").GetComponent<Text>();
        voiceSource = GetComponent<AudioSource>();


        DialogBoxManager.instance.SwitchPerson("All", "off");//�����������

    }


    public void ReadTreatment(string treatmentName, int index)//��ȡ����ĳ�籾��index��
    {
        Debug.Log("��ȡ�¾籾" + treatmentName);
        this.treatmentName = treatmentName;
        this.index = 0;
        Treatment = FileHandler.instance.ReadTxtFile(treatmentName + ".txt");
        DialogBoxManager.instance.SwitchPerson("All", "off");
        ReadTreatmentToIndex(index);


    }
    public int FindIndexByText(string text)
    {
        for (int i = index - 1; i >= 0; i--)
        {
            if (Treatment[i].Split('|').Length < 2)//������ǶԻ���
            {
                continue;
            }
            string treatmentTextInIndex = Treatment[i].Split('|')[1];
            Debug.Log("Ѱ��index����ǰ���ı�Ϊ" + treatmentTextInIndex + "��indexΪ" + i);
            if (text == treatmentTextInIndex)
            {
                return i;
            }
        }
        return 0;
    }

    public void ReadTreatmentToIndex(int index)//�ӵ�0�ж�����index��λ�õ����������,��������ʱ������ʾ��ȷ���棬������ܻ�ȱ������
    {
        for (int i = 0; i < index; i++)//ִ��һ�����е�����
        {
            Debug.Log(i);
            ReadTreatmentLine(i);//��ȡ�ڼ��о籾

        }
        if (index == 0)
        {
            this.index = 0;
        }
        else
        {
            this.index = index;
        }

        Debug.Log("��ȡ����" + this.index + "��");
        ReadTreatmentLine(this.index);
    }




    private void Update()
    {
        //�Զ������㷨˵������ÿ�ζ�ȡ�籾ʱ�����Ӧ�õȴ���ʱ�䣬���ҿ�ʼ��ʱ����ʱ�䡣����Ѿ������Զ����ţ�������Э�̣��ڵ�ʱ����Զ���ȡ��һ�о籾
        //ͬʱ��AutoPlaying��Ϊ�Ƿ���ʹ��Э���Զ����ŵı�־�������ǰ�籾û������Э��(AutoPlaying==false)��˵���ڶ�ȡ����ʱû�п����Զ����š�
        //���������ʱ�����Զ����ţ�Ӧ�ȴ��������������������ʾ����ٶ�ȡ��һ�о籾��Ҳ���ǵ�(playTime>waitAudioTime)ʱ��Ȼ����������ڲ���ĳ�о籾ʱ�ر��Զ�����
        //name��Ӧ�ùر�Э�̣�����AutoPlaying���Ƿ�������Э�̣�����Ϊfalse;
        if (playTime >= 0)
        {
            playTime += Time.deltaTime;
        }
        if (treatmentAutoPlay && !AutoPlaying)
        {
            if (playTime > waitAudioTime)
            {
                playTime = -1;
                ReadTreatmentLine(index);

            }
        }
        if (!treatmentAutoPlay)//ֹͣ���Զ�����
        {
            StopCoroutine("AutoPlay");
            AutoPlaying = false;
        }

    }


    public void ReadTreatmentLine(int line)
    {

        StopCoroutine("AutoPlay");
        playTime = 0;//����ʱ���ʱ

        string tempText = Treatment[line];//��ȡ�籾��

        if (tempText[0] == '#')//��ȡ��ע���У���ȡ��һ��
        {
            Debug.Log(tempText);
            ReadTreatmentLine(line + 1);
            index++;
            return;
        }
        if (tempText[0] == '@')//������
        {
            //����
            string command = tempText.Substring(1);//��ȡ����
            string[] parameters = Regex.Split(command, "\\s+");
            Debug.Log("���" + parameters[0]);


            if (parameters[0] == "TurnBg")//�л�����
            {

                DialogBoxManager.instance.TurnBackground(parameters[1], float.Parse(parameters[2]));

            }
            else if (parameters[0] == "SwitchPerson")
            {
                DialogBoxManager.instance.SwitchPerson(parameters[1], parameters[2]);

            }
            else if (parameters[0] == "AddPerson")
            {
                DialogBoxManager.instance.AddPerson(parameters[1], parameters[2], parameters[3]);
            }
            else if (parameters[0] == "End")
            {
                DialogBoxManager.instance.TurnBackground("����_��", 2);
                DialogBoxManager.instance.SwitchWindow("off");
                End = true;
                return;
            }
            ReadTreatmentLine(line + 1);
            index++;
            return;
        }

        info = tempText.Split('|');
        //Debug.Log("��ȡ��" + line + "��,info[0]="+info[0]+",info[1]="+info[1]+ ",info[2]=" + info[2]);
        currentPersonName = info[0];
        currentTreatmentText = info[1];
        currentPersonPicturePath = info[2];//���������ǡ��ҡ��������˹������ǻ���·����"null"ʱ�����ȡ��������ͼƬ·�����޷�ʹ�õ�
        string voicePath = info[3];
        string picturePosition = info[4];
        //@��ͷ������������


        //ͼƬ·������ͷ��@�Ļ���ʹ���Զ���·��
        if (info[2][0] == '@')
        {
            //�Զ���ͼƬ·��
            currentPersonPicturePath = currentPersonPicturePath.Substring(currentPersonPicturePath.IndexOf('@') + 1);
            DialogBoxManager.instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }

        else if (currentPersonName != "��" && currentPersonPicturePath != "null" && currentPersonName != protagonist)//����ʹ��Ĭ��·����ʾ����
        {
            currentPersonPicturePath = "Pictures/Person/" + currentPersonName + "/" + currentPersonPicturePath;//�������ֲ��ұ���
            oldPersonPicture = currentPersonPicturePath;
            DialogBoxManager.instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }
        else if (currentPersonPicturePath == "null" || currentPersonName == "��" || currentPersonName == protagonist)//��������ǡ��ҡ������˹�����·�����ǿգ��򲻶�����������
        {
            currentPersonPicturePath = oldPersonPicture;
            //������
        }
        else if (currentPersonPicturePath == "hide")//���·������hide������������
        {
            currentPersonPicturePath = "None";//ȫ͸��ͼƬ����������
            DialogBoxManager.instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }

        //��������
        AudioClip voice = (AudioClip)Resources.Load("Voice/" + currentPersonName + "/" + voicePath, typeof(AudioClip));
        voiceSource.clip = voice;
        voiceSource.Play();

        //�Զ������߼�
        if (voicePath != "" && voicePath != "null")//������·��
        {
            waitAudioTime = voiceSource.clip.length + 1;//��ȡ�������ȣ��������������������Զ�����ʱ������������ʱЭ�̣���ʱ���Զ�������һ���Ի���ע��Э�̱����ڶ�ȡ�籾ʱ�رա�

        }
        else
        {
            waitAudioTime = Mathf.CeilToInt((currentTreatmentText.Length / DialogBoxManager.instance.showTextSpeed)) + 2;

        }

        if (treatmentAutoPlay)
        {
            StartCoroutine("AutoPlay");
        }

        //������ʾ
        string leftNameText = currentPersonName;

        //�Ի�����
        if (currentPersonName != "��" && currentPersonName != "")
        {
            leftNameText = "��" + currentPersonName + "��";
            currentTreatmentText = "��" + currentTreatmentText + "��";
        }
        if (currentPersonName == "��")
        {
            leftNameText = "";
        }
        LeftName.text = leftNameText;
        DialogBoxManager.instance.ShowTalkText(currentTreatmentText, DialogBoxManager.instance.showTextSpeed);

        index++;
    }

    IEnumerator AutoPlay()//�Զ�����
    {
        AutoPlaying = true;
        yield return new WaitForSeconds(waitAudioTime);
        AutoPlaying = false;
        ReadTreatmentLine(index);
    }
}
