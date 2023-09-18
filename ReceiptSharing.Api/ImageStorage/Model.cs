#nullable enable
namespace ReceiptSharing.Api.Models{


    public class ResponseIgmur {
       public required ResponseIgmurData Data {get; set;}
    }

    public class ResponseIgmurData {
       public string id {get; set;} = "";

       public string type {get; set;} = "";

       public string link {get; set;} = "";
    }
        
        

}