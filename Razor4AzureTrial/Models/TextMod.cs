using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Razor4AzureTrial.Models
{
    [DataContract]
    public class term
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int OriginalIndex { get; set; }
        [DataMember]
        public int ListId { get; set; }
        [DataMember]
        public string Term { get; set; }
    }

    [DataContract]
    public class email
    {
        [DataMember]
        public string Detected { get; set; }
        [DataMember]
        public string SubType { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Index { get; set; }
    }

    [DataContract]
    public class ipa
    {
        [DataMember]
        public string SubType { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Index { get; set; }
    }

    [DataContract]
    public class phone
    {
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Index { get; set; }
    }

    [DataContract]
    public class address
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Index { get; set; }
    }

    [DataContract]
    public class ssn
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Index { get; set; }
    }

    [DataContract]
    public class pii
    {
        [DataMember]
        public List<email> Email { get; set; }
        [DataMember]
        public List<ipa> IPA { get; set; }
        [DataMember]
        public List<phone> Phone { get; set; }
        [DataMember]
        public List<address> Address { get; set; }
        [DataMember]
        public List<ssn> SNN { get; set; }
    }

    [DataContract]
    public class category
    {
        [DataMember]
        public double score { get; set; }
    }
    
    [DataContract]
    public class classification
    {
        [DataMember]
        public bool ReviewRecommended { get; set; }
        [DataMember]
        public category Category1 { get; set; }
        [DataMember]
        public category Category2 { get; set; }
        [DataMember]
        public category Category3 { get; set; }
    }

    [DataContract]
    public class TextMod
    {
        [DataMember]
        public string OriginalText { get; set; }
        [DataMember]
        public string NormalizedText { get; set; }
        [DataMember]
        public List<string> Misrepresentation { get; set; }
        [DataMember]
        public pii PII { get; set; }
        [DataMember]
        public classification Classification { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public List<term> Terms { get; set; }
        [DataMember]
        public string TrackingId { get; set; }
    }
}
