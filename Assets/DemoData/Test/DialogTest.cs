using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DialogSystem.Data;
using DialogSystem.DataStructure;
using DialogSystem.Utilits;
using UnityEngine;
using UnityEngine.UI;

public class DialogTest : MonoBehaviour
{
    [SerializeField] private DialogTreeData_SO _dataSo;
    private List<ContextInfo>.Enumerator _curcontexts;
    private List<string> _curEventID;
    private List<string>.Enumerator _curSentence;
    private Sprite characterImg;
    private string characterName;


    [SerializeField] private Text DialogSentence;
    [SerializeField] private Image DialogCharacter;
    [SerializeField] private List<string> choicesList;
    [SerializeField] private Text CharacterName;
    [SerializeField] private List<Button> chioceButtons;
    
    private void Start()
    {
        _dataSo.m_Tree.ResetTree();
        UpdateOnce();
    }
    

    private void GetANewNode(string textHeader)
    {
       _dataSo.m_Tree.MoveCurNodeByHeader(textHeader);
    }

    private void UpdateOnce()
    {
        DePacakgeNode(_dataSo.m_Tree.m_CurNode);
        UpdateDialog();
    }

    private bool DePacakgeNode(NodeBase node)
    {
        _curcontexts = node.m_Context.m_contextInfos.GetEnumerator();
        _curcontexts.MoveNext();
        if (_curcontexts.Current.singleSentence != null)
        {
            _curSentence = _curcontexts.Current.singleSentence.GetEnumerator();
            _curSentence.MoveNext();
        }
        characterName = _curcontexts.Current.name.ToString();
        characterImg = _curcontexts.Current.characterImg;
        
        if (node.m_nodeType == DialogNodeType.Dialog)
        {
            if (_curSentence.Current == null)
            {
                return false;
            }
        }
        else
        {
            Debug.Log((node is EventNode)+"" +(node is DialogNode));
            if (node is EventNode enode)
            {
                _curEventID = enode._triggerEvtID;
            }
            else
            {
                Debug.Log("捕获事件节点失败");
            }    
            if (_curEventID == null)
                return false;
        }

        return true;
    }

    private void UpdateDialog()
    {
        DialogCharacter.sprite = characterImg;
        DialogSentence.text = _curSentence.Current;
        CharacterName.text = characterName;
    }

    public void MoveNextSentence()
    {
        _curSentence.MoveNext();
        if (_curSentence.Current == null)
        {
            if (!TryGetNextContext())
            {
                if (!ShowChioce())
                {
                    if (!TriggerEvent())
                    {
                        //下个节点是无条件的或者是最后一个节点
                        if (_dataSo.m_Tree.m_CurNode.m_childIDDic.Count == 0) return;
                        var specialHeader = _dataSo.m_Tree.m_CurNode.m_childIDDic.Keys.GetEnumerator();
                        specialHeader.MoveNext();
                        GetANewNode(specialHeader.Current);
                        UpdateOnce();
                        if (specialHeader.Current == "__EVENTNODE")
                        {
                            CallLater().Forget();
                        }
                    }
                    return;
                }
            }
        }
        
        UpdateDialog();
    }

    private bool TryGetNextContext()
    {
        _curcontexts.MoveNext();
        if (_curcontexts.Current.singleSentence == null) return false;
        
        characterName = _curcontexts.Current.name.ToString();
        characterImg = _curcontexts.Current.characterImg;
        _curSentence = _curcontexts.Current.singleSentence.GetEnumerator();
        _curSentence.MoveNext();
        return true;    
    }

    private bool ShowChioce()
    {
        choicesList = new List<string>();
        foreach (var pair in _dataSo.m_Tree.m_CurNode.m_childIDDic)
        {
            choicesList.Add(pair.Key);                
        }

        if (choicesList.Count == 1)
        {
            if (choicesList[0] == "__EVENTNODE" || choicesList[0] == "")
            {
                return false;
            }
        }
        for (int i = 0; i < choicesList.Count; ++i)
        {
            chioceButtons[i].transform.GetChild(0).GetComponent<Text>().text = choicesList[i];
        }
        
        return true;
    }

    public void ClickButton(Text textHeader)
    {
        GetANewNode(textHeader.text);
        UpdateOnce();
        foreach (var btn in chioceButtons)
        {
            btn.transform.GetChild(0).GetComponent<Text>().text = "";
        }
    }
    
    private bool TriggerEvent()
    {
        if (_curEventID == null)
            return false;
        foreach (var id in _curEventID)
        {
            UnityEventCenter.CallReciverByID(id);
        }
        return true;
    }

    private async UniTaskVoid CallLater()
    {
        await UniTask.WaitForSeconds(1f);
        TriggerEvent();
    }
    
}
