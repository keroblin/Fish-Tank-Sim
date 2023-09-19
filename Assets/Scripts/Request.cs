using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request : ScriptableObject
{
    public float budget;
    public Tag style;
    public Tag fishType;
    public Sprite portrait;
    public string requestName;
    public string characterName;

    [Multiline]
    public string requestText;

    public string GenerateRequest()
    {
        string request = requestText;
        request = request.Replace("[style]",style.name);
        request = request.Replace("[fishType]", fishType.name);
        request = request.Replace("[budget]", "£" + budget.ToString("#.00"));
        return request;
    }

    public string GenerateResponse(Tank ctx)
    {
        //need to figure out sentence endings maybe?

        string response = "";
        /*string dirtinessComment;
        string styleComment;
        string harmonyComment;*/
        string closingWords = "";
        int score = 5;

        if(ctx.tankDirtiness > 2)
        {
            //say its a little dirty
            response += "Ooh its a little dirty";
            score -= 2;
        }
        else if(ctx.tankDirtiness > 4)
        {
            //say its filthy
            score -= 3;
            response += "WOW that is filthy!";
        }

        if(ctx.mostCommonStyle == style)
        {
            response += " You got the " + style.name + " vibe spot on!";
            score += 2;
        }
        else
        {
            response += " ..Oh you didn't get the style quite right...";
            score += 1;
        }


        if(ctx.tankHarmony > 4.5)
        {
            response += " The fish seem to love it here!";
            score += 2;
        }
        else if(ctx.tankHarmony > 3.5)
        {
            response += " The fish seem pretty happy!";
            score += 1;
        }
        else if(ctx.tankHarmony > 2.5)
        {
            response += " Theres a little tension between the fish";
            score -= 1;
        }
        else
        {
            response += " The fish seem real sad";
            score -= 2;
        }

        if(ctx.value > budget)
        {
            response += "Oh. I mean this is over budget..";
            if(score > 4)
            {
                response += " but its pretty nice, I'll pay up";
            }
            else
            {
                response += " I'm not sure I can afford that...";
                score -= 2;
            }
        }


        if(score >= 5)
        {
            response += "\nThis is amazing! You know you deserve more clients, I'll spread the word for ya!";
        }
        else if(score > 3)
        {
            response += "\nThis is pretty good! Thanks for your time";
        }
        else if(score > 1.7)
        {
            response += "\nIt'll do. I guess.";
        }
        else if(score <= 0)
        {
            response += "\nSorry, I can't take this. Maybe another time.";
        }

        return response;
    }
}
