using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDGame.UI.RichText;
using System.Text.RegularExpressions;

public class RichTextTest : MonoBehaviour {
    public RichText text; 
	// Use this for initialization
	void Start () {
        text.onClick.AddListener((Dictionary<string, string> param) => {
           foreach(KeyValuePair<string,string> val in param)
            {
                Debug.Log("key: " + val.Key + " val: " + val.Value);
            }
        });

        //var strJson = @"<a param=link=123><color=red>[测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试]测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试]测试][测试][测试][测试][测试][测试][测试][测试][测试测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试]</color></a>";
        var strJson = @"<a param=link abc=123>[测试][测试]</a>";
        //方法一
        string _spriteTagPattern = @"<a(?:\s+(\w+)\s*=\s*(?<quota>['""]?)([\w\/]+)\k<quota>)+\s*\>(?<text>(?:(?!</?a\b).)*)</a>";
       // string _spriteTagPattern = @"<a(?:\s+(\w+)\s*=\s*(?<quota>['""]?)([\w\/]+)\k<quota>)+\s*\>(.*?)</a>";
        Regex rg = new Regex(_spriteTagPattern);
        MatchCollection mc = rg.Matches(strJson);
        foreach (Match match in mc)
        {
            var keyCaptures = match.Groups[1].Captures;
            var valCaptures = match.Groups[2].Captures;

            Debug.Log(match.Groups["text"].Value);

            var count = keyCaptures.Count;
            if (count != valCaptures.Count)
            {
                return ;
            }
            Debug.Log(match.Groups[2].Value);
            for (int i = 0; i < keyCaptures.Count; ++i)
            {
                var key = keyCaptures[i].Value;
                var val = valCaptures[i].Value;
                Debug.Log("Key: " + key + " Val: " + val);
            }
        }
           

        //方法三
        //         foreach (Match match in Regex.Matches(strJson, "<a[^>]*>([^<]*)</a>"))
        //             Debug.Log(string.Format("Duplicate '{0}' found at position {1}.",  match.Groups[1].Value, match.Groups[2].Index));

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}            
