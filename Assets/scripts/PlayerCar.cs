using UnityEngine;
public class PlayerCar : MonoBehaviour
{
    [SerializeField] GameObject Player;
    bool InCar;
    GameObject car;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!InCar)
            {
                Collider[] c = Physics.OverlapSphere(Player.transform.position, 5f);
                if (c != null && c.Length > 0)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i].tag == "Car1")
                        {
                            Player.SetActive(false);
                            Player.transform.SetParent(c[i].transform);
                            c[i].transform.parent.GetComponent<SCC_Drivetrain>().enabled = true;
                            c[i].transform.parent.GetComponentInChildren<Camera>().enabled = true;
                            Player.GetComponentInChildren<Camera>().enabled = false;
                            car = c[i].gameObject;
                            InCar = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                InCar = false;
                Player.SetActive(true);
                Player.transform.SetParent(null);
                car.transform.parent.GetComponent<SCC_Drivetrain>().enabled = false;
                car.transform.parent.GetComponentInChildren<Camera>().enabled = false;
                Player.GetComponentInChildren<Camera>().enabled = true;
            }
        }
    }
}