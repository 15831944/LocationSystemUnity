using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FacilityStringHelper{

    const string suffix = "...";
    /// <summary>
    /// 末尾省略
    /// </summary>
    /// <param name="input">输入的文本</param>
    /// <param name="maxWidth">中间插入的标志(...)</param>
    /// <param name="text">文本框</param>
    /// <returns></returns>
    public static string StripLengthWithSuffix(string input, int maxWidth, Text text)
    {
        int suffixWidth = CalculateLengthOfText(suffix, text);
        int len = CalculateLengthOfText(input, text);
        //截断text的长度
        if (len > maxWidth)
        {
            return StripLength(input, text, maxWidth - suffixWidth) + suffix;
        }
        else
        {
            return input;
        }
    }
    /// <summary>
    /// 中间省略
    /// </summary>
    /// <param name="input">输入的文本</param>
    /// <param name="insertSymbol">中间插入的标志(...)</param>
    /// <param name="maxSize">最大显示尺寸（一般为Text.width）</param>
    /// <param name="textTemp">文本框</param>
    /// <returns></returns>
    public static string StripTextMiddle(string input, int maxSize, Text textTemp)
    {
        int textLenght = CalculateLengthOfText(input, textTemp);
        if (textLenght > maxSize)
        {
            return StripTextMiddle(input, suffix, textLenght, maxSize, textTemp);
        }
        else
        {
            return input;
        }
    }

    /// <summary>
    /// 中间添加省略号
    /// </summary>
    /// <returns></returns>
    static string StripTextMiddle(string input, string insertSymbol, int textLength, int maxSize, Text textTemp)
    {
        try
        {
            if(input.Contains("搅拌器电机"))
            {
                int xxx = 0;
            }
            int insertSymbolWidth = CalculateLengthOfText(insertSymbol, textTemp);
            int startLength = (maxSize - insertSymbolWidth) / 2;
            int endLenght = textLength - startLength;

            Font myFont = textTemp.font;  //chatText is my Text component
            myFont.RequestCharactersInTexture(input, textTemp.fontSize, textTemp.fontStyle);

            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = input.ToCharArray();
            int totalLength = 0;
            int startIndex = 0;
            int endIndex = 0;
            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, textTemp.fontSize);
                int newLength = totalLength + characterInfo.advance;
                if (newLength <= startLength)
                {
                    startIndex++;
                }
                if (newLength <= endLenght)
                {
                    endIndex++;
                }
                else
                {
                    break;
                }
                totalLength += characterInfo.advance;
            }            
            string value= string.Format("{0}{1}{2}", input.Substring(0, startIndex), insertSymbol, input.Substring(endIndex));
            Debug.Log(string.Format("StartIndex:{0}  EndIndex:{1}  Input:{2}  NewLength:{3}", startIndex, endIndex, input,value));
            return value;
        }
        catch (Exception e)
        {
            Debug.Log("StripText Error:" + e.ToString());
            return input;
        }
    }


    /// <summary>
    /// 根据maxWidth来截断input拿到子字符串
    /// </summary>
    /// <param name="input"></param>
    /// <param name="maxWidth"></param>
    /// <returns></returns>
    static string StripLength(string input, Text text, int maxWidth)
    {
        int totalLength = 0;
        Font myFont = text.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(input, text.fontSize, text.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = input.ToCharArray();
        int i = 0;
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);

            int newLength = totalLength + characterInfo.advance;
            if (newLength > maxWidth)
            {
                if (Mathf.Abs(newLength - maxWidth) > Mathf.Abs(maxWidth - totalLength))
                {
                    break;
                }
                else
                {
                    totalLength = newLength;
                    i++;
                    break;
                }
            }
            totalLength += characterInfo.advance;
            i++;
        }
        return input.Substring(0, i);
    }

    /// <summary>
    /// 计算字符串在指定text控件中的长度
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    static int CalculateLengthOfText(string message, Text text)
    {
        int totalLength = 0;
        Font myFont = text.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(message, text.fontSize, text.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();

        char[] arr = message.ToCharArray();

        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);

            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}
