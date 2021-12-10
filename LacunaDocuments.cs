using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  CERTEDUC.EnvioLote
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Folder
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentId")]
        public object ParentId { get; set; }
    }

    public class Actions
    {
        [JsonProperty("signerId")]
        public object SignerId { get; set; }

        [JsonProperty("approverId")]
        public object ApproverId { get; set; }

        [JsonProperty("signRuleId")]
        public object SignRuleId { get; set; }
    }

    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("folder")]
        public Folder Folder { get; set; }

        [JsonProperty("organization")]
        public object Organization { get; set; }

        [JsonProperty("type")]
        public object Type { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("pendingForUser")]
        public bool PendingForUser { get; set; }

        [JsonProperty("actions")]
        public Actions Actions { get; set; }
    }

    public class LacunaDocuments
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public object Phone { get; set; }
    }

    public class FlowAction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("notarizationStatus")]
        public object NotarizationStatus { get; set; }

        [JsonProperty("notarizationDate")]
        public object NotarizationDate { get; set; }

        [JsonProperty("notarizationDescription")]
        public object NotarizationDescription { get; set; }

        [JsonProperty("shouldNotarize")]
        public bool ShouldNotarize { get; set; }

        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("pendingDate")]
        public DateTime? PendingDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("numberRequiredSignatures")]
        public int NumberRequiredSignatures { get; set; }

        [JsonProperty("ruleName")]
        public object RuleName { get; set; }

        [JsonProperty("title")]
        public object Title { get; set; }

        [JsonProperty("signRuleUsers")]
        public object SignRuleUsers { get; set; }

        [JsonProperty("marks")]
        public object Marks { get; set; }

        [JsonProperty("allowElectronicSignature")]
        public bool? AllowElectronicSignature { get; set; }

        [JsonProperty("requireSmsAuthenticationToSignElectronically")]
        public bool? RequireSmsAuthenticationToSignElectronically { get; set; }

        [JsonProperty("requireAuthenticatorAppToSignElectronically")]
        public bool? RequireAuthenticatorAppToSignElectronically { get; set; }

        [JsonProperty("requireSelfieAuthenticationToSignElectronically")]
        public bool? RequireSelfieAuthenticationToSignElectronically { get; set; }

        [JsonProperty("requireDatavalidAuthenticationToSignElectronically")]
        public bool? RequireDatavalidAuthenticationToSignElectronically { get; set; }

        [JsonProperty("requirePixAuthenticationToSignElectronically")]
        public bool? RequirePixAuthenticationToSignElectronically { get; set; }

        [JsonProperty("refusalReason")]
        public object RefusalReason { get; set; }
    }

    public class Permissions
    {
        [JsonProperty("move")]
        public bool Move { get; set; }

        [JsonProperty("editFlow")]
        public bool EditFlow { get; set; }

        [JsonProperty("viewSignedDocumentWithExternalFlowBeforeCompleted")]
        public bool ViewSignedDocumentWithExternalFlowBeforeCompleted { get; set; }

        [JsonProperty("submitExternalFlowFiles")]
        public bool SubmitExternalFlowFiles { get; set; }

        [JsonProperty("updateDocumentVersion")]
        public bool UpdateDocumentVersion { get; set; }

        [JsonProperty("createAttachments")]
        public bool CreateAttachments { get; set; }

        [JsonProperty("deleteAttachments")]
        public bool DeleteAttachments { get; set; }

        [JsonProperty("viewPrivateAttachments")]
        public bool ViewPrivateAttachments { get; set; }

        [JsonProperty("editAttachments")]
        public bool EditAttachments { get; set; }

        [JsonProperty("createMeeting")]
        public bool CreateMeeting { get; set; }

        [JsonProperty("deleteMeeting")]
        public bool DeleteMeeting { get; set; }
    }

    public class Pending
    {
        [JsonProperty("signerId")]
        public object SignerId { get; set; }

        [JsonProperty("signRuleId")]
        public object SignRuleId { get; set; }

        [JsonProperty("approverId")]
        public object ApproverId { get; set; }
    }

    public class Organization
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("owner")]
        public object Owner { get; set; }
    }

    public class CreatedBy
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class LacunaDetailDocument
    {
        [JsonProperty("checksumMd5")]
        public string ChecksumMd5 { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("flowActions")]
        public List<FlowAction> FlowActions { get; set; }

        [JsonProperty("observers")]
        public List<object> Observers { get; set; }

        [JsonProperty("notaryType")]
        public object NotaryType { get; set; }

        [JsonProperty("numberOfExtraPagesToNotarize")]
        public object NumberOfExtraPagesToNotarize { get; set; }

        [JsonProperty("files")]
        public object Files { get; set; }

        [JsonProperty("attachments")]
        public object Attachments { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }

        [JsonProperty("pending")]
        public Pending Pending { get; set; }

        [JsonProperty("notifiedEmails")]
        public object NotifiedEmails { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("qrCode")]
        public string QrCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("fileSize")]
        public int FileSize { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("hasSignature")]
        public bool HasSignature { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isConcluded")]
        public bool IsConcluded { get; set; }

        [JsonProperty("folder")]
        public Folder Folder { get; set; }

        [JsonProperty("organization")]
        public Organization Organization { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty("type")]
        public object Type { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("hasExternalFlow")]
        public bool HasExternalFlow { get; set; }

        [JsonProperty("forceCadesSignature")]
        public bool ForceCadesSignature { get; set; }

        [JsonProperty("isScanned")]
        public bool IsScanned { get; set; }
    }

    public class Pendente
    {
        public string Nome;

        public int quantidade;

        public string documentId;

        public string flowActionId;
    }

    public class URLRetorno
    {
        public string url { get; set; }
        public string embedUrl { get; set; }
    }


}