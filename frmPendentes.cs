using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace  CERTEDUC.EnvioLote
{
    public partial class frmPendentes : Form
    {
        private string tokenAPI;

        private List<Pendente> listaPendentesExt;


        public frmPendentes(string _tokenAPI)
        {
            InitializeComponent();



            this.tokenAPI = _tokenAPI;

            this.listaPendentesExt = new List<Pendente>();
        }


        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnVerificar.Enabled = false;
                Application.UseWaitCursor = true;

                this.barraProgresso.Value = 0;

                string folderID = this.txtFolderID.Text;

                var client = new RestClient("https://asp.assinaturasempapel.com.br/api/documents?IsConcluded=false&FolderId=" + folderID + @"&Limit=10000");


                //var client = new RestClient("https://asp.assinaturasempapel.com.br/api/documents?IsConcluded=false&FolderId=b49691ad-05b8-4a09-6db3-08d93d53ffee&Limit=10000");


                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-Api-Key", this.tokenAPI);
                IRestResponse response = client.Execute(request);

                LacunaDocuments documentos = JsonConvert.DeserializeObject<LacunaDocuments>(response.Content);

                int pendenteDuasAss = 0;
                int pendenteUmaAss = 0;
                int completos = 0;

                List<Pendente> listaPendentesInt = new List<Pendente>();

                this.listaPendentesExt = new List<Pendente>();

                if (documentos != null && documentos.Items != null && documentos.Items.Count > 0)
                {
                    this.barraProgresso.Maximum = documentos.Items.Count;

                    foreach (Item documento in documentos.Items)
                    {
                        //var clientItem = new RestClient("https://asp.assinaturasempapel.com.br/api/documents/0057def3-d3a8-403d-9fe1-8b22ce4e080a");

                        string link = @"https://asp.assinaturasempapel.com.br/api/documents/" + documento.Id;

                        var clientItem = new RestClient(link);
                        client.Timeout = -1;
                        var requestItem = new RestRequest(Method.GET);
                        requestItem.AddHeader("X-Api-Key", this.tokenAPI);
                        IRestResponse responseItem = clientItem.Execute(requestItem);

                        if (responseItem != null && !string.IsNullOrEmpty(responseItem.Content))
                        {
                            LacunaDetailDocument detalhamentoDoDocumento = JsonConvert.DeserializeObject<LacunaDetailDocument>(responseItem.Content);

                            if (detalhamentoDoDocumento != null)
                            {
                                if (detalhamentoDoDocumento.FlowActions != null && detalhamentoDoDocumento.FlowActions.Count > 0)
                                {
                                    int nEtapa = 0;

                                    bool aPrimeiraEstaPendente = false;

                                    detalhamentoDoDocumento.FlowActions = detalhamentoDoDocumento.FlowActions.OrderBy(c => c.Step).ToList();

                                    foreach (FlowAction acao in detalhamentoDoDocumento.FlowActions)
                                    {
                                        nEtapa++;

                                        if (acao.Status == "Pending" && nEtapa == 1)
                                        {
                                            this.txtLog2.Text += acao.User.Name + Environment.NewLine;

                                            pendenteDuasAss++;

                                            aPrimeiraEstaPendente = true;

                                            Pendente pExt = new Pendente();

                                            pExt.Nome = acao.User.Name;
                                            pExt.documentId = documento.Id;
                                            pExt.flowActionId = acao.Id;

                                            listaPendentesExt.Add(pExt);
                                        }
                                        else if (acao.Status == "Pending" && nEtapa == 2 && !aPrimeiraEstaPendente)
                                        {
                                            pendenteUmaAss++;

                                            bool achou = false;

                                            foreach (Pendente pendente in listaPendentesInt)
                                            {
                                                if (pendente.Nome == acao.User.Name)
                                                {
                                                    achou = true;

                                                    pendente.quantidade++;
                                                }
                                            }

                                            if (!achou)
                                            {
                                                Pendente pendenteNovo = new Pendente();

                                                pendenteNovo.Nome = acao.User.Name;
                                                pendenteNovo.quantidade = 1;

                                                listaPendentesInt.Add(pendenteNovo);
                                            }
                                        }
                                        else if (acao.Status == "Completed" && nEtapa == 2 && !aPrimeiraEstaPendente)
                                        {
                                            completos++;
                                        }
                                    }
                                }
                            }
                        }

                        this.barraProgresso.Value++;

                        int progresso = ((100 * this.barraProgresso.Value) / this.barraProgresso.Maximum);

                        this.lblPorCento.Text = progresso.ToString() + " %";

                        Application.DoEvents();
                    }

                    this.lblNExt.Text = pendenteDuasAss.ToString();
                    this.lblNInt.Text = pendenteUmaAss.ToString();
                    this.lblNCompleto.Text = completos.ToString();

                    Application.DoEvents();

                    foreach (Pendente pendenteInterno in listaPendentesInt)
                    {
                        this.txtLog1.Text += pendenteInterno.Nome + "-" + pendenteInterno.quantidade + Environment.NewLine;
                        Application.DoEvents();
                    }

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.btnVerificar.Enabled = true;
                Application.UseWaitCursor = false;
                Application.DoEvents();
            }
        }

        private void btnNotificar_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnVerificar.Enabled = false;
                this.btnNotificar.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();

                if (this.listaPendentesExt != null && this.listaPendentesExt.Count > 0)
                {
                    this.barraProgresso.Value = 0;

                    this.barraProgresso.Maximum = listaPendentesExt.Count;

                    foreach (Pendente itemPend in this.listaPendentesExt)
                    {
                        this.EnviarNotificacao(itemPend.documentId, itemPend.flowActionId);

                        this.barraProgresso.Value++;

                        int progresso = ((100 * this.barraProgresso.Value) / this.barraProgresso.Maximum);

                        this.lblPorCento.Text = progresso.ToString() + " %";

                        Application.DoEvents();
                    }

                    MessageBox.Show("Notificações enviadas com Sucesso!", "Ok", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Não existem assinaturas pendentes para notificação.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.btnVerificar.Enabled = true;
                this.btnNotificar.Enabled = true;
                Application.UseWaitCursor = false;
                Application.DoEvents();
            }
        }

        private void EnviarNotificacao(string documenttID, string flowID)
        {
            var client = new RestClient("https://asp.assinaturasempapel.com.br/api/notifications/flow-action-reminder");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("X-Api-Key", tokenAPI);
            request.AddHeader("Content-Type", "application/json");

            var body = @"{" + "\n" +
               @"    ""documentId"": """ + documenttID + @""",
                        " + "\n" +
               @"    ""flowActionId"": """ + flowID + @"""
                        " + "\n" +
               @"}";

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private void btnGerarLinks_Click(object sender, EventArgs e)
        {
            try
            {

                string nomesLinks = string.Empty;

                this.btnVerificar.Enabled = false;
                Application.UseWaitCursor = true;

                this.barraProgresso.Value = 0;

                string folderID = this.txtFolderID.Text;

                var client = new RestClient("https://asp.assinaturasempapel.com.br/api/documents?IsConcluded=false&FolderId=" + folderID + @"&Limit=10000");


                //var client = new RestClient("https://asp.assinaturasempapel.com.br/api/documents?IsConcluded=false&FolderId=b49691ad-05b8-4a09-6db3-08d93d53ffee&Limit=10000");


                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-Api-Key", this.tokenAPI);
                IRestResponse response = client.Execute(request);

                LacunaDocuments documentos = JsonConvert.DeserializeObject<LacunaDocuments>(response.Content);

                int pendenteDuasAss = 0;
                int pendenteUmaAss = 0;
                int completos = 0;

                List<Pendente> listaPendentesInt = new List<Pendente>();

                this.listaPendentesExt = new List<Pendente>();

                if (documentos != null && documentos.Items != null && documentos.Items.Count > 0)
                {
                    this.barraProgresso.Maximum = documentos.Items.Count;

                    foreach (Item documento in documentos.Items)
                    {
                        //var clientItem = new RestClient("https://asp.assinaturasempapel.com.br/api/documents/0057def3-d3a8-403d-9fe1-8b22ce4e080a");

                        string link = @"https://asp.assinaturasempapel.com.br/api/documents/" + documento.Id;

                        var clientItem = new RestClient(link);
                        client.Timeout = -1;
                        var requestItem = new RestRequest(Method.GET);
                        requestItem.AddHeader("X-Api-Key", this.tokenAPI);
                        IRestResponse responseItem = clientItem.Execute(requestItem);

                        if (responseItem != null && !string.IsNullOrEmpty(responseItem.Content))
                        {
                            LacunaDetailDocument detalhamentoDoDocumento = JsonConvert.DeserializeObject<LacunaDetailDocument>(responseItem.Content);

                            if (detalhamentoDoDocumento != null)
                            {
                                if (detalhamentoDoDocumento.FlowActions != null && detalhamentoDoDocumento.FlowActions.Count > 0)
                                {
                                    int nEtapa = 0;

                                    bool aPrimeiraEstaPendente = false;

                                    detalhamentoDoDocumento.FlowActions = detalhamentoDoDocumento.FlowActions.OrderBy(c => c.Step).ToList();

                                    foreach (FlowAction acao in detalhamentoDoDocumento.FlowActions)
                                    {
                                        nEtapa++;

                                        if (acao.Status == "Pending" && nEtapa == 1)
                                        {
                                            pendenteDuasAss++;

                                            aPrimeiraEstaPendente = true;

                                            Pendente pExt = new Pendente();

                                            pExt.Nome = acao.User.Name;
                                            pExt.documentId = documento.Id;
                                            pExt.flowActionId = acao.Id;

                                            listaPendentesExt.Add(pExt);

                                            var clientURL = new RestClient("https://asp.assinaturasempapel.com.br/api/documents/" + documento.Id + "/action-url");
                                            clientURL.Timeout = -1;
                                            var requestURL = new RestRequest(Method.POST);
                                            requestURL.AddHeader("X-Api-Key", tokenAPI);
                                            requestURL.AddHeader("Content-Type", "application/json");
                                            var body = @"{
                                            " + "\n" +
                                            @"    ""identifier"": """ + acao.User.Identifier + @""",
                                            " + "\n" +
                                            @"    ""emailAddress"": """ + acao.User.Email + @"""
                                            " + "\n" +
                                            @"}";
                                            requestURL.AddParameter("application/json", body, ParameterType.RequestBody);
                                            IRestResponse responseURL = clientURL.Execute(requestURL);

                                            URLRetorno responseURLClass = JsonConvert.DeserializeObject<URLRetorno>(responseURL.Content);

                                            //this.txtLog2.Text += documento.Name + " - Link: " + responseURLClass.url;

                                            nomesLinks += documento.Name + " - Link: " + responseURLClass.url + Environment.NewLine;
                                        }
                                        else if (acao.Status == "Pending" && nEtapa == 2 && !aPrimeiraEstaPendente)
                                        {
                                            //pendenteUmaAss++;

                                            //bool achou = false;

                                            //foreach (Pendente pendente in listaPendentesInt)
                                            //{
                                            //    if (pendente.Nome == acao.User.Name)
                                            //    {
                                            //        achou = true;

                                            //        pendente.quantidade++;
                                            //    }
                                            //}

                                            //if (!achou)
                                            //{
                                            //    Pendente pendenteNovo = new Pendente();

                                            //    pendenteNovo.Nome = acao.User.Name;
                                            //    pendenteNovo.quantidade = 1;

                                            //    listaPendentesInt.Add(pendenteNovo);
                                            //}
                                        }
                                        else if (acao.Status == "Completed" && nEtapa == 2 && !aPrimeiraEstaPendente)
                                        {
                                            //completos++;
                                        }
                                    }
                                }
                            }
                        }

                        this.barraProgresso.Value++;

                        int progresso = ((100 * this.barraProgresso.Value) / this.barraProgresso.Maximum);

                        this.lblPorCento.Text = progresso.ToString() + " %";

                        Application.DoEvents();
                    }

                    this.lblNExt.Text = pendenteDuasAss.ToString();
                    this.lblNInt.Text = pendenteUmaAss.ToString();
                    this.lblNCompleto.Text = completos.ToString();

                    this.txtLog2.Text += nomesLinks;

                    Application.DoEvents();

                    foreach (Pendente pendenteInterno in listaPendentesInt)
                    {
                        this.txtLog1.Text += pendenteInterno.Nome + "-" + pendenteInterno.quantidade + Environment.NewLine;
                        Application.DoEvents();
                    }

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.btnVerificar.Enabled = true;
                Application.UseWaitCursor = false;
                Application.DoEvents();
            }
        }

        private void frmPendentes_Load(object sender, EventArgs e)
        {

        }
    }
}