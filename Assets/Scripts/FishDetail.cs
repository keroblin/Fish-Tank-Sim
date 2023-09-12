using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishDetail : ShopDetail
{
    //probs just make this the fishstatview kinda
    //public whateverthefishstatviewiscalled view
    public TextMeshProUGUI compatibility;
    public override void Set(Purchasable purchasable)
    {
        Fish fish = purchasable as Fish;
        //fishstatview.set(fish)
        compatibility.text = "Compatibility: " + (fish.CalculateCompat() * 100f).ToString("#.00") + "%";
    }
}
