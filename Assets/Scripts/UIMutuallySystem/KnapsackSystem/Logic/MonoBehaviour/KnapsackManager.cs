using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class KnapsackManager : SingletonMono<KnapsackManager>
{
    public class DargData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    //TODO:最后添加模版用于保存数据
    [Header("Knapsack Data")]
    public KnapsackData_SO knapsackData;

    public KnapsackData_SO actionData;

    public KnapsackData_SO equipmentData;

    public CharacterBaseInfo_SO characterBaseInfo;


    [Header("ContainerS")]
    public ContainerUI knapsackUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Darg Canvas")]
    public Canvas dargCanvas;
    public DargData currentDarg;

    
    private bool isOpen = false;
    [Header("UI Panel")]
    [SerializeField] private GameObject knapsackPanel;
    [SerializeField] private GameObject characterStatePanel;

    [Header("TextMeshPro")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI armorText;

    private void Start()
    {
        knapsackUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
        characterBaseInfo = HpPanelManager.Instance.playerStateInfo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            knapsackPanel.SetActive(isOpen);
            characterStatePanel.SetActive(isOpen);
        }
        UpdateCharacterInfo();
    }

    public bool CheckKnapsackUI(Vector3 position)
    {
        for (int i = 0; i < knapsackUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = knapsackUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = actionUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }


    public bool CheckEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = equipmentUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }


    public void UpdateCharacterInfo()
    {
        healthText.text = characterBaseInfo.currentHealth.ToString();
        attackText.text = characterBaseInfo.attack.ToString();
        armorText.text = characterBaseInfo.defense.ToString();
    }
}
