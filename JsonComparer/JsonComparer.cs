using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.ComponentModel;

 

[Category("Custom C# Code")]
[DisplayName("Json Comparer")]
[Description("Compare Two Json Strings and Return Diff as Json String")]
public class JsonComparer : CodeActivity
{

    [Category("Input")]
    [DisplayName("First JsonString")]
    [Description("Enter the first Json String")]
    [RequiredArgument]
    public InArgument<String> FirstJsonString { get; set; }

    [Category("Input")]
    [DisplayName("Second JsonString")]
    [Description("Enter the Second Json String")]
    [RequiredArgument]
    public InArgument<String> SecondJsonString { get; set; }

    public OutArgument<String> ResultString { get; set; }
    protected override void Execute(CodeActivityContext context)
        {
            String JsonStr1 = FirstJsonString.Get(context);
            String JsonStr2 = SecondJsonString.Get(context);
            JObject J1 = JObject.Parse(JsonStr1);
            JObject J2 = JObject.Parse(JsonStr2);
            JObject J1Leafs = JsonComparer.GetLeafs(J1);
            JObject J2Leafs = JsonComparer.GetLeafs(J2);
            
            JToken diffJsonNested = JsonComparer.GetJsonDifference(J1Leafs,  J2Leafs);

            ResultString.Set(context, diffJsonNested.ToString());
        }
    

    public static string CompareJsonStrings(string json1, string json2)
    {
        JObject jsonObject1 = JObject.Parse(json1);
        JObject jsonObject2 = JObject.Parse(json2);
        
        JToken diff = GetJsonDifference(jsonObject1, jsonObject2);
        
        if(diff != null){
        return diff.ToString();
        }
        else{
            return "";
        }
    }
    
    public static JToken GetJsonDifference(JToken token1, JToken token2)
    {

        //check if either token is null then return
        if(token1 is null || token2 is null){
            return null;
        }

        //if Json strings are same then return null
        if (JToken.DeepEquals(token1, token2))
        {
            return null;
        }
        
        //if type of Json token is different return both values as strings
        if (token1.Type != token2.Type)
        {
            return token1.ToString() + ", " + token2.ToString();
        }
        
        //check if tokens are both basic values, and then compare values and return values as comma separated string if different else return null
        if (token1 is JValue value1 && token2 is JValue value2)
        {
            return !JToken.DeepEquals(value1, value2) ? value1.ToString() + ", " + value2.ToString() : null;
        }
        
        //check if both tokens are arrays and if so go through array and call getdiff on each item in array, return array of diffs
        if (token1 is JArray array1 && token2 is JArray array2)
        {
            JArray diffArray = new JArray();
            
            for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
            {
                JToken diff = GetJsonDifference(i < array1.Count ? array1[i] : null, i < array2.Count ? array2[i] : null);
                if (diff != null)
                {
                    
                    diffArray.Add(diff);
                }
            }
            
            return diffArray;
        }
        
        //if tokens are both Json strings then go through each property of second string and call getDiff 
        if (token1 is JObject obj1 && token2 is JObject obj2)
        {
            JObject diffObj = new JObject();
            
            var properties = obj2.Properties();
            
            foreach (var property in properties)
            {
                JToken diff = GetJsonDifference(obj1[property.Name], property.Value);
                if (diff != null)
                {
                    diffObj.Add(property.Name, diff);
                }
            }
            
            return diffObj;
        }
        
        return token2;
    }

 public static JObject GetLeafs(JObject jobject){
    JObject RetObj = new JObject();
     var properties = jobject.Properties();
     foreach (var property in properties)
            {
                var value = property.Value;

                // if value is a Json string itself then add the Leafs of those values to the returnObj
                if(value is JObject jobjInner){;
                    JObject LeafObj = GetLeafs(jobjInner);
                    foreach(var innerproperty in LeafObj.Properties()){
                        RetObj.Add(innerproperty.Name,innerproperty.Value); 
                    }
                }
                // if value is a JValue aka a leaf then add it to returnObj
                if(value is JValue){
                    RetObj.Add(property.Name,value); 
                }
                //if value is an array then loop through array and get leafs of each element and add their values to returnObj
                if (value is JArray){
                    foreach(var item in value){
                        if(item is JObject innerJObject){
                            JObject LeafObj = GetLeafs(innerJObject);
                            foreach(var innerproperty in LeafObj.Properties()){
                             if(RetObj[innerproperty.Name] == null){
                                RetObj.Add(innerproperty.Name,innerproperty.Value); 
                                }
                            }
                        }
                        if(item is JValue){
                            if(RetObj[property.Name] == null){
                                RetObj.Add(property.Name,value);
                            }
                        }
                    }

                }
            }
    return RetObj;
 }

}

