using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Request", menuName = "ScriptableObjects/Request")]
public class Request : ScriptableObject //maybe make it a class so we can randomly generate them? I kinda wanna write them as characters tho
{
    public float budget;
    public float lengthInTicks;
    public Tag style;
    public Fish specificFishRequest;
    public Item specificItemRequest;

    public Sprite portrait;
    public string requestName;
    public string characterName;
    public string[] dirtResponse = { "Its a lil dirty..", "EW. Sorry its just.. a bit gross" };
    public string[] styleResponse = { "You got the [style] vibe spot on!", "Ah you didn't quite get the [style] style I was looking for.." };
    public string[] harmonyResponse = { "The fish look super happy!", "The fish are getting along well", "The fish look a bit agitated", "The fish seem really angry" };
    public string[] budgetResponse = { "Oop its a little over budget...", "But I'm willing to pay", "I can't really afford that, sorry" };
    public string gotSpecificFish = "Oh hey you got in my favourite fish, the [specificFish]! Thanks so much!";
    public string gotSpecificItem = "Cheers for putting in the [specificItem]!!";

    public string[] scoreResponse = {"This is amazing! I''l pay double!", "Good job, I'll pay a lil extra ;)", "Cool stuff, thanks!", "I can fix it up..."};

    public string cancelResponse;
    public string outOfTimeResponse;
    public string requestText = "Hey can I have a [style] tank? I would like to see a [specificFish] in there too, and a [specificItem]. I have a budget of [budget]. Thanks!";

    public string GenerateRequest(string textToParse)
    {
        textToParse = textToParse.Replace("[style]",style.name);
        textToParse = textToParse.Replace("[specificFish]", specificFishRequest.name);
        textToParse = textToParse.Replace("[specificItem]", specificItemRequest.name);
        textToParse = textToParse.Replace("[budget]", "£" + budget.ToString("#.00"));
        return textToParse;
    }
}
