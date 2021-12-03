using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject crosshair_idle;
    [SerializeField] GameObject crosshair_attack;

    [SerializeField]TMP_Text healthText;
    [SerializeField]TMP_Text magazineText;
  
    float fireCounter;

    AK47 ak47;
    Deagle deagle;
    public Dummy selectedDummy;
    GameObject activeWeapon;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ak47 = GameObject.Find("AK47").GetComponent<AK47>();
        deagle = GameObject.Find("DesertEagle").GetComponent<Deagle>();

        activeWeapon = GameObject.Find("AK47");
        deagle.gameObject.SetActive(false);
    }

    void Update()
    {
        //Selecting the Dummy in front of camera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            crosshair_attack.SetActive(true);
            crosshair_idle.SetActive(false);

            selectedDummy = hit.transform.gameObject.GetComponent<Dummy>();
        }

        else //Deselecting dummy
        {
            crosshair_attack.SetActive(false);
            crosshair_idle.SetActive(true);
            selectedDummy = null;
        }
        fireCounter += Time.deltaTime;

        //Fire Deagle (Semi-Automatic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && selectedDummy != null && !selectedDummy.isDead) 
            if (activeWeapon.name == "DesertEagle")
            {
                if(deagle.GetMagazine() != 0)
                {
                    int x = deagle.Fire(fireCounter);
                    if (x == 2) selectedDummy.TakeDamage(deagle.Damage);
                }
                else
                {
                    Debug.Log("Magazine is empty. Reload.");
                }
            }
        }

        //Fire AK47 (Automatic)
        if (Input.GetKey(KeyCode.Mouse0) && selectedDummy != null && !selectedDummy.isDead) 
        {
            if (activeWeapon.name == "AK47")
            {
                if(ak47.GetMagazine() != 0)
                {   
                    int x = ak47.Fire(fireCounter);                
                    if (x == 2) selectedDummy.TakeDamage(ak47.Damage);
                }
                else
                {
                    Debug.Log("Magazine is empty. Reload.");
                }
            }
        }

        //Recoil Stop
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            activeWeapon.GetComponent<IGun>().FireRecoilStop();   
        }

        //Change the weapon
        else if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if(activeWeapon.name == "AK47") //Current was AK47
            {
                Debug.Log("Active Weapon: Desert Eagle");
                ak47.gameObject.SetActive(false);
                deagle.gameObject.SetActive(true);

                activeWeapon = GameObject.Find("DesertEagle");

            }

            else
            {
                Debug.Log("Active Weapon: AK47");
                deagle.gameObject.SetActive(false);
                ak47.gameObject.SetActive(true);

                activeWeapon = GameObject.Find("AK47");
            }
        }
        
        //Revive the target
        else if (Input.GetKeyDown(KeyCode.T)) 
        {
            selectedDummy.RegenerateDummy();
        }

        //Reload
        else if (Input.GetKeyDown(KeyCode.R)) 
        {
            activeWeapon.GetComponent<IGun>().Reload();   
        }

        //UI
        magazineText.text =": " + activeWeapon.GetComponent<IGun>().GetMagazine().ToString();           
    }

    public void ResetCounter()
    {
        fireCounter = 0;
    }
}
