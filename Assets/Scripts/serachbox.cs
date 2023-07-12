using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class serachbox : MonoBehaviour
{
    public List<GameObject> t_countrycode;
    public List<string> t_afterserachcode;
    public Text t_searcfieldtext;
    private void Start()
    {
        t_afterserachcode.Clear();
    }
    public void onvaluechange(string input)
    {
        if (input == "")
        {
            t_countrycode.ForEach(x => x.gameObject.SetActive(true));
            return;
        }


        foreach (var item in t_countrycode)
        {

           
            item.gameObject.SetActive(true);
          
            Text serach=item.GetComponentInChildren<Text>();
            if (serach.text.ToLower().Contains(input.ToLower()) && !string.IsNullOrEmpty(input))
            {
                var afterserach = serach.text;
                GameObject gameObject =new GameObject( afterserach);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
           
        }


    }

  
}
