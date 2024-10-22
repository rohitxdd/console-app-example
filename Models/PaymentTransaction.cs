using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reconsile.Models
{
    public class PaymentTransaction
    {
        [JsonPropertyName("tid")]
        public int Tid { get; set; }

        [JsonPropertyName("orderno")]
        public string OrderNo { get; set; }

        [JsonPropertyName("tdate")]
        public string TDate { get; set; }

        [JsonPropertyName("sbiepayrefid")]
        public string SbiePayRefId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("paymode")]
        public string PayMode { get; set; }

        [JsonPropertyName("otherdetails")]
        public string OtherDetails { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("bankcode")]
        public string BankCode { get; set; }

        [JsonPropertyName("bankrefno")]
        public string BankRefNo { get; set; }

        [JsonPropertyName("banktdate")]
        public string BankTDate { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("cin")]
        public string Cin { get; set; }
    }
}
