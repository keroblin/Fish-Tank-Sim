using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class RequestBehaviour : MonoBehaviour
{
    public Request request;

    public RequestSelector selector;

    public Button requestManager;
    public GameObject requestView;
    public Image portrait;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI requestUI;
    public Button submit;
    public Button cancel;
    public GameObject visuals;
    public Slider timeLeft;

    public GameObject reviewScreen;
    public TextMeshProUGUI reviewText;
    public TextMeshProUGUI earningsText;
    public Animator anim;
    public string animQuery;

    public int score = 0;
    public float earnings;

    private void Start()
    {
        if (Manager.Instance.currentTank.assignedRequest)
        {
            Set(Manager.Instance.currentTank.assignedRequest);
        }
    }

    public void Set(Request _request)
    {
        request = _request;
        submit.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        portrait.sprite = request.portrait;
        charName.text = request.characterName;
        requestUI.text = request.GenerateRequest(request.requestText);
        submit.onClick.AddListener(SubmitTankRequest);
        cancel.onClick.AddListener(CancelTankRequest);
        timeLeft.maxValue = request.lengthInTicks;
        timeLeft.value = request.lengthInTicks;
        Manager.Instance.currentTank.onTankTick.AddListener(UpdateTimer);
    }

    public void UpdateTimer()
    {
        if(timeLeft.value - 1 > 0)
        {
            timeLeft.value--;
        }
        else
        {
            Manager.Instance.currentTank.onTankTick.RemoveListener(UpdateTimer);
            Speak(request.outOfTimeResponse);
        }
    }

    public IEnumerator Speak(string response)
    {
        //read a line, wait at \n, read another line, wait at \n until done
        string[] lines = response.Split("\n");
        reviewText.text = response;
        reviewText.maxVisibleCharacters = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > 2) //gross workaround to check if the string is empty of alphabet characters
            {
                Manager.Instance.universalSFX.PlayOneShot(Manager.Instance.talkSound);
            }
            reviewText.maxVisibleCharacters += lines[i].Length+1;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    public IEnumerator Review()
    {
        reviewScreen.SetActive(true);
        anim.Play("ReviewIn");
        yield return Speak(GenerateResponse(Manager.Instance.currentTank));
        yield return new WaitForSeconds(.5f);
        earningsText.text = "Earnings: £" + earnings.ToString("#.00");
        Manager.Instance.currentMoney += earnings;
        anim.Play(animQuery);
        requestManager.gameObject.SetActive(false);
        requestView.SetActive(false);
        selector.gameObject.SetActive(true);
    }

    public string GenerateResponse(Tank ctx)
    {
        //need to figure out sentence endings maybe?

        string response = "";

        if (ctx.tankDirtiness > 2)
        {
            //say its a little dirty
            response += request.dirtResponse[0] + "\n";
        }
        else if (ctx.tankDirtiness > 4)
        {
            //say its filthy
            if (score - 1 >= 0)
            {
                score -= 1;
            }
            else
            {
                score = 0;
            }
            response += request.dirtResponse[1] + "\n";
        }
        if (ctx.mostCommonStyle == request.style)
        {
            response += request.styleResponse[0].Replace("[style]", request.style.name) + "\n";
            if (score + 1 <= 5)
            {
                score += 1;
            }
            else
            {
                score = 5;
            }
        }
        else
        {
            response += request.styleResponse[1].Replace("[style]", request.style.name) + "\n";
        }

        if (ctx.assignedPlaceables.Find(x => x.purchasable == request.specificFishRequest))
        {
            response += request.gotSpecificFish.Replace("[specificFish]", request.specificFishRequest.name) + "\n";
            if (score + 1 <= 5)
            {
                score += 1;
            }
            else
            {
                score = 5;
            }
        }
        if (ctx.assignedPlaceables.Find(x => x.purchasable == request.specificItemRequest))
        {
            response += request.gotSpecificItem.Replace("[specificItem]", request.specificItemRequest.name) + "\n";
            if (score + 1 <= 5)
            {
                score += 1;
            }
            else
            {
                score = 5;
            }
        }

        if (ctx.tankHarmony > 4.5)
        {
            response += request.harmonyResponse[0] + "\n";
            if (score + 2 <= 5)
            {
                score += 2;
            }
            else
            {
                score = 5;
            }
        }
        else if (ctx.tankHarmony > 3.5)
        {
            response += request.harmonyResponse[1] + "\n";
            if (score + 1 <= 5)
            {
                score += 1;
            }
            else
            {
                score = 5;
            }
        }
        else if (ctx.tankHarmony > 2.5)
        {
            response += request.harmonyResponse[2] + "\n";
        }
        else
        {
            response += request.harmonyResponse[3] + "\n";
            if (score - 1 >= 0)
            {
                score -= 1;
            }
            else
            {
                score = 0;
            }
        }

        if (ctx.value > request.budget)
        {
            response += request.budgetResponse[0].Replace("[budget]", "£" + request.budget.ToString("#.00")) + "\n";
            if (score > 4 && ctx.value - request.budget < request.budget + (request.budget / 2))
            {
                response += request.budgetResponse[1].Replace("[budget]", "£" + request.budget.ToString("#.00")) + "\n";
            }
            else
            {
                response += request.budgetResponse[2].Replace("[budget]", "£" + request.budget.ToString("#.00")) + "\n";
                if (score - 1 >= 0)
                {
                    score -= 1;
                }
                else
                {
                    score = 0;
                }
            }
        }

        response += "\n";
        if (score >= 5)
        {
            response += request.scoreResponse[0];
            earnings = request.budget + (request.budget/2); //double pay
            animQuery = "Perfect";
        }
        else if (score > 3)
        {
            response += request.scoreResponse[1];
            earnings = request.budget + (request.budget / 3); //50% extra
            animQuery = "Good";
        }
        else if (score > 0)
        {
            response += request.scoreResponse[2];
            earnings = request.budget + (request.budget / 4); //standard pay, budget + a quarter
            animQuery = "Ok";
        }
        else if (score <= 0)
        {
            response += request.scoreResponse[3];
            earnings = request.budget - (request.budget/2); //half refund
            animQuery = "Bad";
        }
        Debug.Log("Score was " + score);
        Debug.Log("Anim was " + animQuery);
        return response;
    }
    //maybe put this in a different helper script
    public void PlaySound(AudioClip clip)
    {
        Manager.Instance.universalSFX.PlayOneShot(clip);
    }

    public void SubmitTankRequest()
    {
        Debug.Log("Submitted");
        StartCoroutine(Review());

        //spawn a new tank
        //remove all items by telling their placeables to get rid of themselves
        //tell manager to make a new tank, i dont want that behaviour here
        //once its done, open the curtain
        //send stuff to request ui to turn on and off

    }

    public void ExitReview()
    {
        StartCoroutine("WaitForNewTank");
        reviewScreen.SetActive(false);
        earningsText.text = "";
    }

    IEnumerator WaitForNewTank()
    {
        Curtain.Instance.CurtainDown("Moving tank out", 100f);
        Manager.Instance.currentTank.onTankTick.RemoveListener(UpdateTimer);
        Manager.Instance.currentTank.ResetTank();
        //put the request select back on
        //put the request manager off
        yield return new WaitForSeconds(5f); //eventually this will awaiting be a coroutine that does a lot more loading things
        Curtain.Instance.CurtainUp();
    }

    public void CancelTankRequest()
    {
        //show a 'poof' effect on the request
        Manager.Instance.currentTank.onTankTick.AddListener(UpdateTimer);
        anim.Play("CancelRequest");
        request = null;
        //tell the request selector to become active
    }
}
