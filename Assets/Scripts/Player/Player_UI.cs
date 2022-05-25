using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;

//This class holds logic for the dungeon scene UI
//leveling display and skil display and miscell 
public class Player_UI : NetworkBehaviour
{

    public GameObject uiRoot; //set this true if localAuthority

    //--------------------Xp UI-----------------------------------------
    public GameObject xpBarRoot;
    
    public Slider xpSlider;
    private Text xpLevelText;
    private Text xpPercentageText;
    public float percentage;
    private void getXPUI()
    {
        xpSlider = xpBarRoot.GetComponent<Slider>();
        xpLevelText = xpBarRoot.transform.GetChild(3).GetComponent<Text>();
        xpPercentageText = xpBarRoot.transform.GetChild(2).GetComponent<Text>();
    }


    //------------Health UI--------------------------------------
    public GameObject healthBarRoot;
    private Slider healthBarSlider;
    private Text healthText;
    private void getHealthUI()
    {
        healthBarSlider = healthBarRoot.GetComponent<Slider>();
        healthText = healthBarRoot.transform.GetChild(2).GetComponent<Text>();
    }

    //------------------Buff UI----------------------------------
    public GameObject buffRoot;
    private GameObject[] buffCooldowns;
    private Image[] buffImage;
    private void getBuffUI()
    {
        buffCooldowns = new GameObject[20];
        buffImage = new Image[20];
        for (int i=0; i<20; i++)
        {
            buffCooldowns[i] = buffRoot.transform.GetChild(i).gameObject;
            buffImage[i] = buffRoot.transform.GetChild(i).GetChild(1).GetComponent<Image>();
        }
    }

    //---------------------Skill UI ------------------------------
    public GameObject leftSkillRoot, rightSkillRoot;
    private Image leftSkill, rightSkill;
    private Image leftSkillPicture, rightSkillPicture;
    private GameObject leftSkillCover, rightSkillCover;
    public Color cooldownColor, durationColor; //color for skill cooldown and duration
    private void getSkillUI()
    {
        leftSkill = leftSkillRoot.GetComponent<Image>();
        rightSkill = rightSkillRoot.GetComponent<Image>();

        leftSkillPicture = leftSkillRoot.transform.GetChild(1).GetComponent<Image>();
        rightSkillPicture = rightSkillRoot.transform.GetChild(1).GetComponent<Image>();

        leftSkillCover = leftSkillRoot.transform.GetChild(2).gameObject;
        rightSkillCover = rightSkillRoot.transform.GetChild(2).gameObject;
    }



    //Keys UI
    public Image[] keys;
    public Image bossKey;

    //Coins Count
    public Text coinsText;

    //Shield UI/Graphics
    public UnityEngine.UI.Image shieldHoldImg, shieldCooldownImg;


    //Refrences
    private Player_Inventory inv;
    private Player_Skills skillManager;
    private Player_Combat playerCombat;
    private Player_Buffs playerBuffs;
    
    //Grab references
    void Awake()
    {
        try {
            skillManager = this.GetComponent<Player_Skills>();
            inv = this.GetComponent<Player_Inventory>();
            playerCombat = this.GetComponent<Player_Combat>();
            playerBuffs = this.GetComponent<Player_Buffs>();
        }
        catch(Exception e)
        {
            Debug.LogError("There was a problem finding the PlayerUIManager Refernces.");
            Debug.LogError(e);
        }

       
    }


    private void Start()
    {
        if (!hasAuthority) { return; }
        uiRoot.SetActive(true);
        getXPUI();
        getHealthUI();
        getBuffUI();
        getSkillUI();
        //Grab left and right skill picture
        if (inv.skills[0] is SkillObjectScript)
        {
            leftSkillPicture.sprite = inv.skills[0].skillIcon;

        }
        else
        {
            leftSkillPicture.sprite = null;
        }


        if (inv.skills[1] is SkillObjectScript)
        {
            rightSkillPicture.sprite = inv.skills[1].skillIcon;
        }
        else
        {
            rightSkillPicture.sprite = null;
        }

    }

    //This method updates the shield UI using player input
    [Client]
    private void shieldUIUpdates()
    {
        if (!hasAuthority) { return; }
        //line the UI up with where the player is
        shieldHoldImg.gameObject.transform.position = this.transform.position;
        shieldCooldownImg.gameObject.transform.position = this.transform.position;

        //not holding the shield
        if (!playerCombat.getShieldInfo().holding_shield)
        {
            //cooling down, DARK BLUE
            if (playerCombat.getShieldInfo().shieldHoldTimer < playerCombat.getShieldInfo().shieldHoldTimerTotal)
            {
                shieldHoldImg.fillAmount = playerCombat.getShieldInfo().shieldHoldTimer / playerCombat.getShieldInfo().shieldHoldTimerTotal;
            }
            else
            {
                shieldHoldImg.fillAmount = 0f / playerCombat.getShieldInfo().shieldHoldTimerTotal;
            }
        }

        //Holding shield and still have shield stamina
        if (playerCombat.getShieldInfo().holding_shield && !playerCombat.getShieldInfo().shield_Tired)
        {
            //DARK BLUE
            if (playerCombat.getShieldInfo().shieldHoldTimer >= 0f)
            {
                shieldHoldImg.fillAmount = playerCombat.getShieldInfo().shieldHoldTimer / playerCombat.getShieldInfo().shieldHoldTimerTotal;
            }
        }

        //If we run out of shield stamina we get shield tired, Show the light blue cooldown (while player is slowed)
        if (playerCombat.getShieldInfo().shield_Tired)
        {
            if (playerCombat.getShieldInfo().shieldCoolTimer >= 0f)
            {
                shieldCooldownImg.fillAmount = playerCombat.getShieldInfo().shieldCoolTimer / playerCombat.getShieldInfo().shieldCoolTimerTotal;
            }
        }
    }

    //This method updates the health bar UI
    //NOTE: May add effects such as: flash drop
    [Client]
    private void healthbarUIUpdates()
    {
        //HealthBar
        healthText.text = (int)playerCombat.getHealth() + "/" + (int)playerCombat.getMaxHealth();
        healthBarSlider.value = (playerCombat.getHealth() / playerCombat.getMaxHealth());
    }




    //This method updates the xp bar UI
    [Client]
    private void xpbarUIUpdate()
    {
        xpLevelText.text = "Level " + inv.level; //level count
        xpPercentageText.text = (int)((xpSlider.value) * 100) + "%"; //xp percent
        xpSlider.value = Mathf.Lerp(xpSlider.value, xpSlider.value + percentage, Time.deltaTime * 3f); //slider
        if (xpSlider.value >= inv.currentXpAmt / inv.currentXpCap)
        {
            percentage = 0f;
        }
    }

    //This method is used to slowly count up the xp bar to the amt we earned
    [Client]
    public IEnumerator countUpXPBar(float amt)
    {
        //whats the percentage chunk
        for (int i = 0; i < amt; i++)
        {
            percentage += (i / inv.currentXpCap);
            yield return new WaitForSeconds(.002f);
        }


    }




    //Updates the UI for the keys we currently have in our inventory
    [Client]
    private void keysUIUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            if (inv.keys[i])
            {
                keys[i].gameObject.SetActive(true);
            }
            else
            {
                keys[i].gameObject.SetActive(false);
            }
        }
        if (inv.bossKey)
        {
            bossKey.gameObject.SetActive(true);
        }
        else
        {
            bossKey.gameObject.SetActive(false);
        }
    }

    //Called every frame
    private void Update()
    {
        if (!hasAuthority) { return; }
        shieldUIUpdates();

        healthbarUIUpdates();

        xpbarUIUpdate();

        coinsText.text = "" + inv.coins; //update coin ui

        displayBuffs(); //display active buffs

        displaySkills(); //display skill ui

        keysUIUpdate(); //keys and boss key

    }

    //This method udates the skill UI, cooldowns, and shows if the skill is active (greyed out)
    [Client]
    private void displaySkills()
    {
        //left
        if (skillManager.skill1Started)
        {
            leftSkillCover.gameObject.SetActive(true);
            leftSkill.fillMethod = Image.FillMethod.Vertical;
            leftSkill.fillAmount = skillManager.skill1Duration / skillManager.skill1TotalDur;
            leftSkill.color = durationColor;

        }
        if (skillManager.skill1Started == false && skillManager.skill1Ready == false)
        {
            //show cooldown
            leftSkill.fillMethod = Image.FillMethod.Radial360;
            leftSkill.fillAmount = skillManager.skill1Cooldown / skillManager.skill1TotalCool;
            leftSkill.color = cooldownColor;
        }

        if (skillManager.skill1Ready)
        {
            leftSkillCover.gameObject.SetActive(false);
            leftSkill.fillAmount = 0f;
        }


        //right
        if (skillManager.skill2Started)
        {
            rightSkillCover.gameObject.SetActive(true);
            rightSkill.fillMethod = Image.FillMethod.Vertical;
            rightSkill.fillAmount = skillManager.skill2Duration / skillManager.skill2TotalDur;
            rightSkill.color = durationColor;

        }
        if (skillManager.skill2Started == false && skillManager.skill2Ready == false)
        {
            //show cooldown
            rightSkill.fillMethod = Image.FillMethod.Radial360;
            rightSkill.fillAmount = skillManager.skill2Cooldown / skillManager.skill2TotalCool;
            rightSkill.color = cooldownColor;
        }

        if (skillManager.skill2Ready)
        {
            rightSkillCover.gameObject.SetActive(false);
            rightSkill.fillAmount = 0f;
        }
    }

    //This method displays the active buffs for the player onscreen
    [Client]
    private void displayBuffs()
    {
        List<Player_Buffs.ActiveBuffDebuff> activeBuffs = playerBuffs.activeBuffsDebuffs;
        int i = 0;
        foreach(Player_Buffs.ActiveBuffDebuff item in activeBuffs)
        {
            if(item.buffDebuff is BuffDebuff)
            {
                buffCooldowns[i].SetActive(true);
                buffCooldowns[i].GetComponent<Image>().fillAmount = item.duration / item.totalDuration;
                print(item.buffDebuff.buffName);
                if (item.buffDebuff.buffName != null)
                {
                    buffImage[i].sprite = inv.db.buffImageByName(item.buffDebuff.buffName);
                }
                if (i >= 19)
                {
                    //all slots are used
                    Debug.LogError("All slots for active buffs are being used, replacing the first buff image with this one. Note that the buffs all still remain active, just the visual is changed");
                    //rotate back to first slot
                    i = 0;
                }
                else
                {
                    i++;
                }
               
            }
            
        }
        if (i >= 19)
        {
            //all slots are used
        }
        else
        {
            for(int j=i; j<20; j++)
            {
                buffCooldowns[j].SetActive(false);
            }
        }


    }

}
