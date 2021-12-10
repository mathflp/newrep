using Lacuna.Signer.Api;
using Lacuna.Signer.Api.DocumentMark;
using Lacuna.Signer.Api.Documents;
using Lacuna.Signer.Api.FlowActions;
using Lacuna.Signer.Api.Observers;
using Lacuna.Signer.Api.Users;
using Lacuna.Signer.Client;
using RestSharp;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CERTEDUC.EnvioLote
{
    public class LacunaEnviarDocComOrdemDeAssinaturas
    {
        string g_token, g_link, g_folderId;
        float x_ass, y_ass;
        bool posAssinatura;
        protected SignerClient SignerClient;

        protected TextBox txtLogSucesso;
        protected TextBox txtLogErro;

        public LacunaEnviarDocComOrdemDeAssinaturas(string link, string token, TextBox _txtLogSucesso, TextBox _txtLogErro, string p_folderId, bool p_posAssinatura)
        {
            SignerClient = new SignerClient(link, token);
            g_token = token;
            g_link = link + "api/";
            g_folderId = p_folderId;
            posAssinatura = p_posAssinatura;
            // x_ass[] = p_x_ass;
            //y_ass[] = p_y_ass;
            this.txtLogSucesso = _txtLogSucesso;
            this.txtLogErro = _txtLogErro;
        }

        public async Task EnviaDoc(string arquivoDoc, List<PessoaFluxo> listaPessoasFluxo, List<string> p_listaNotifiedEmails)
        {
            var filePath = arquivoDoc;
            var fileName = Path.GetFileName(filePath);

            try
            {
                var file = File.ReadAllBytes(filePath);
                var uploadModel = await SignerClient.UploadFileAsync(fileName, file, "application/pdf");

                var fileUploadModel = new FileUploadModel(uploadModel) { DisplayName = fileName };

                List<FlowActionCreateModel> listaFluxo = new List<FlowActionCreateModel>();
                List<ObserverCreateModel> listaObservador = new List<ObserverCreateModel>();
                //List<string> listaNotifiedEmails = new List<string>();


                int contAssPos = 0;
                foreach (PessoaFluxo itemPessoaFluxo in listaPessoasFluxo)
                {
                    ParticipantUserModel participantUser = new ParticipantUserModel();

                    participantUser.Identifier = itemPessoaFluxo.cpf;
                    participantUser.Name = itemPessoaFluxo.nome;
                    participantUser.Email = itemPessoaFluxo.email;
                    participantUser.Phone = itemPessoaFluxo.telefone;


                    //Observador
                    if (itemPessoaFluxo.tipo == FlowActionType.SignRule)
                    {
                        ObserverCreateModel observerX = new ObserverCreateModel()
                        {
                            User = participantUser
                        };

                        listaObservador.Add(observerX);
                    }
                    else if (itemPessoaFluxo.tipo == FlowActionType.Approver) // Aprovador
                    {
                        FlowActionCreateModel flowActionX = new FlowActionCreateModel()
                        {
                            Type = itemPessoaFluxo.tipo,
                            User = participantUser,
                            Step = itemPessoaFluxo.ordem,
                            AllowElectronicSignature = itemPessoaFluxo.permitirAssEletronica,
                            Title = itemPessoaFluxo.titulo
                        };

                        listaFluxo.Add(flowActionX);
                    }
                    else // Assinador
                    {
                        if (posAssinatura)
                        {
                            PageOrientation orientacao = PageOrientation.RETRATO;

                            string assinador = "Ass" + (contAssPos + 1);
                            PdfDocument doc = new PdfDocument();

                            doc.LoadFromFile(filePath);
                            PdfTextFind[] results = null;

                            int nPaginas = doc.Pages.Count;

                            foreach (PdfPageBase page in doc.Pages)
                            {
                                //results = page.FindText(assinador).Finds;
                                results = page.FindText(assinador, TextFindParameter.None).Finds;

                                foreach (PdfTextFind text in results)
                                {
                                    PointF p = text.Position;

                                    //if (page.Rotation == PdfPageRotateAngle.RotateAngle90 || page.Rotation == PdfPageRotateAngle.RotateAngle270)
                                    //{
                                    //    x_ass = p.X;
                                    //    y_ass = p.Y;
                                    //}
                                    //else
                                    //{
                                    x_ass = p.X;
                                    y_ass = p.Y;
                                    //  }
                                }
                            }



                            FlowActionCreateModel flowActionX = new FlowActionCreateModel()
                            {
                                Type = itemPessoaFluxo.tipo,
                                User = participantUser,
                                Step = itemPessoaFluxo.ordem,
                                AllowElectronicSignature = itemPessoaFluxo.permitirAssEletronica,
                                Title = itemPessoaFluxo.titulo,
                                PrePositionedMarks = new List<PrePositionedDocumentMarkModel>
                                {
                                    new PrePositionedDocumentMarkModel()
                                    {
                                        Type = DocumentMarkType.SignatureVisualRepresentation, //This is the attribute responsible for defining the Type of signature you are going to use
                                        UploadId = fileUploadModel.Id, //Document id
                                        TopLeftX = x_ass, //Signature position, in pixels, over the X axis
                                        TopLeftY = y_ass, //Signature position, in pixels, over the Y axis
                                       // Width = 100.0,    //Width of the rectagular where signature will be placed in (It already has a default value)
                                       // Height = 50.0,    //Height of the rectagular where signature will be placed in (It already has a default value)
                                        PageNumber = nPaginas   //Page where the signature wil be placed
                                    }
                                }
                            };
                            contAssPos++;
                            listaFluxo.Add(flowActionX);
                        }
                     else
                    {
                        FlowActionCreateModel flowActionX = new FlowActionCreateModel()
                        {
                            Type = itemPessoaFluxo.tipo,
                            User = participantUser,
                            Step = itemPessoaFluxo.ordem,
                            AllowElectronicSignature = itemPessoaFluxo.permitirAssEletronica,
                            Title = itemPessoaFluxo.titulo,

                        };
                        listaFluxo.Add(flowActionX);
                    }
                    //else
                    //{

                    //    List<ParticipantUserModel> listRule = new List<ParticipantUserModel>();

                    //    ParticipantUserModel pUser1 = new ParticipantUserModel();
                    //    pUser1.Email = "f.henrique@certeduc.com.br";
                    //    pUser1.Name = "fernando";

                    //    listRule.Add(pUser1);

                    //    ParticipantUserModel pUser2 = new ParticipantUserModel();
                    //    pUser1.Email = "d.velasco@grupocermob.com.br";
                    //    pUser1.Name = "Davi";

                    //    listRule.Add(pUser2);




                    //    FlowActionCreateModel flowActionX = new FlowActionCreateModel()
                    //    {
                    //        Type = itemPessoaFluxo.tipo,
                    //        User = participantUser,
                    //        Step = itemPessoaFluxo.ordem,
                    //        AllowElectronicSignature = itemPessoaFluxo.permitirAssEletronica,
                    //        Title = itemPessoaFluxo.titulo

                    //    };
                    //    listaFluxo.Add(flowActionX);
                    //}

                }
            } //Fim Foreach

                string idDoDocumento;
            if (p_listaNotifiedEmails != null && p_listaNotifiedEmails.Count > 0)
            {
                var documentRequest = new CreateDocumentRequest()
                {
                    Files = new List<FileUploadModel>() { fileUploadModel },
                    FlowActions = listaFluxo,
                    Observers = listaObservador,
                    NotifiedEmails = p_listaNotifiedEmails
                };
                var result = (await SignerClient.CreateDocumentAsync(documentRequest)).First();
                idDoDocumento = result.DocumentId.ToString();
            }
            else
            {
                var documentRequest = new CreateDocumentRequest()
                {
                    Files = new List<FileUploadModel>() { fileUploadModel },
                    FlowActions = listaFluxo,
                    Observers = listaObservador
                };
                var result = (await SignerClient.CreateDocumentAsync(documentRequest)).First();
                idDoDocumento = result.DocumentId.ToString();
            }

            try
            {
                //Move documento para a pasta selecionada
                var client = new RestClient(g_link + "documents/" + idDoDocumento + "/folder");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                request.AddHeader("X-Api-Key", g_token);
                request.AddHeader("Content-Type", "application/json");

                var body = @"{" + "\n" +
                   @"    ""folderId"": """ + g_folderId + @""",
                        " + "\n" +
                   @"}";

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                //if (response.StatusCode.ToString() == "OK")
                //{
                //    MessageBox.Show("ok");
                //}
                //else
                //{
                //    MessageBox.Show("fail");
                //}
            }
            catch (Exception)
            {
                throw;
            }


            if (this.txtLogSucesso.InvokeRequired)
            {
                this.txtLogSucesso.Invoke((MethodInvoker)delegate ()
                {
                    this.txtLogSucesso.Text += fileName + " - " + idDoDocumento + Environment.NewLine;

                });
            }
            else
            {
                this.txtLogSucesso.Text += fileName + " - " + idDoDocumento + Environment.NewLine;
            }

            Application.DoEvents();
        }
            catch (Exception ex)
            {
                if (this.txtLogErro.InvokeRequired)
                {
            this.txtLogErro.Invoke((MethodInvoker)delegate ()
            {
                this.txtLogErro.Text += fileName + " - " + ex.Message + Environment.NewLine;

            });
        }
                else
                {
            this.txtLogErro.Text += fileName + " - " + ex.Message + Environment.NewLine;
        }

        Application.DoEvents();
            }
}
    }

    public enum PageOrientation
{
    RETRATO = 0,
    PAISAGEM = 1
}

}