using ClosedXML.Excel;
using Lacuna.Signer.Api;
using Lacuna.Signer.Api.Folders;
using Lacuna.Signer.Client;
using Lacuna.Spa.Api;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CERTEDUC.EnvioLote
{
    public partial class frmGestao : Form
    {
        private string tokenAPI;
        private string link = @"";
        private int totalLinhas = 0;
        private List<Folder> listFolders;
        private List<Pendentes> listPendentes;
        private List<Concluidos> listConcluidos;
        private List<OrdemAcao> listOrdemAcao;
        private List<Organizacao> ListOrgConfig;
        //private List<CerteducOrganizacao> organizacoes;
        //dynamic jsonObj;
        ClsConfig jsonConfig;

        public frmGestao()
        {
            InitializeComponent();

            listFolders = new List<Folder>();
            listPendentes = new List<Pendentes>();
            listConcluidos = new List<Concluidos>();
            listOrdemAcao = new List<OrdemAcao>();
            ListOrgConfig = new List<Organizacao>();
            // organizacoes = new List<CerteducOrganizacao>();
            DesabilitaBtnExecutar();
            LerJson();

            Esconde_tabPages();
            LimpaDetalhamentos();
            // tabPageEnvio.Visible = false;

        }

        private void Esconde_tabPages()
        {
            tabControlGestao.Controls.Remove(tabPageEnvio);
            tabControlGestao.Controls.Remove(tabPageDoc);
            tabControlGestao.Controls.Remove(tabPageOrg);
        }

        private void LerJson()
        {
            this.cmbOrganizacao.Items.Clear();
            this.cmbOrgEnvio.Items.Clear();

            jsonConfig = JsonConvert.DeserializeObject<ClsConfig>(File.ReadAllText(Application.StartupPath + @"\config.json"));

            if (jsonConfig.link.Length > 0)
            {
                lblTitle.Text = jsonConfig.nome.ToString();

                switch (lblTitle.Text.ToUpper())
                {
                    case "TECHCERT":
                        {
                            pictureBoxLogo.Image = Properties.Resources.logo_techcert;
                        }
                        break;
                    case "FECAP":
                        {
                            pictureBoxLogo.Image = Properties.Resources.logo_fecap;
                        }
                        break;
                    case "CNSP":
                        {
                            pictureBoxLogo.Image = Properties.Resources.logo_cnsp;
                        }
                        break;
                    case "BOM JESUS":
                        {
                            pictureBoxLogo.Image = Properties.Resources.logo_bomjesus;
                        }
                        break;
                    case "ASP":
                        {
                            pictureBoxLogo.Image = Properties.Resources.logoAsp;
                        }
                        break;
                    default:
                        pictureBoxLogo.Image = Properties.Resources.logo_certeduc;
                        break;
                }

                string[] subs = jsonConfig.cor_primaria.ToString().Split(',');
                int r_P = Int32.Parse(subs[0]);
                int g_P = Int32.Parse(subs[1]);
                int b_P = Int32.Parse(subs[2]);
                panelCabecalho.BackColor = System.Drawing.Color.FromArgb(r_P, g_P, b_P);
                btnEnvioDocumentos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(r_P, g_P, b_P);
                btnDocumentos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(r_P, g_P, b_P);
                btnOrganizacao.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(r_P, g_P, b_P);


                string[] subsS = jsonConfig.cor_secundaria.ToString().Split(',');
                int r = Int32.Parse(subsS[0]);
                int g = Int32.Parse(subsS[1]);
                int b = Int32.Parse(subsS[2]);
                panelMenu.BackColor = System.Drawing.Color.FromArgb(r, g, b);
                btnEnvioDocumentos.BackColor = System.Drawing.Color.FromArgb(r, g, b);
                btnDocumentos.BackColor = System.Drawing.Color.FromArgb(r, g, b);
                btnOrganizacao.BackColor = System.Drawing.Color.FromArgb(r, g, b);

                link += jsonConfig.link.ToString();


                if (jsonConfig.organizacoes.Count > 0)
                {
                    foreach (var org in jsonConfig.organizacoes)
                    {
                        this.cmbOrganizacao.Items.Add(org.NOME);
                        this.cmbOrgEnvio.Items.Add(org.NOME);
                    }
                }
            }
            else
            {
                MessageBox.Show("A aplicação não encontrou um link e por isso será encerrada. Favor configure o arquivo config.json com um link válido.");
                this.Close();
            }
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnRestaurar.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnRestaurar.Visible = true;
            btnMaximizar.Visible = false;
        }
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDocumentos_Click(object sender, EventArgs e)
        {
            DesabilitaBtnExecutar();
            lblTitle.Text = "Documentos";
            Esconde_tabPages();
            tabControlGestao.Controls.Add(tabPageDoc);
        }

        private void btnEnvioDocumentos_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "Envio de Documentos";
            Esconde_tabPages();
            tabControlGestao.Controls.Add(tabPageEnvio);
        }

        private void btnOrganizacao_Click(object sender, EventArgs e)
        {
            DesabilitaBtnExecutar();
            lblTitle.Text = "Organizações";
            Esconde_tabPages();
            tabControlGestao.Controls.Add(tabPageOrg);

            if (dgvOrganizacoes.RowCount <= 1)
            {
                foreach (var org in jsonConfig.organizacoes)
                {
                    dgvOrganizacoes.Rows.Add(false, org.NOME.ToString(), org.CHAVE.ToString());
                }
            }
            Application.DoEvents();
        }

        private void cmbOrganizacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tokenAPI = "";
            try
            {
                this.prgDocumento.Value = 0;

                foreach (Organizacao org in this.jsonConfig.organizacoes)
                {
                    if (org.NOME == cmbOrganizacao.Text)
                    {
                        this.tokenAPI = org.CHAVE;
                        break;
                    }
                }

                dgvConcluidos.DataSource = null;
                dgvPendentes.DataSource = null;
                var client = new RestClient(link + "folders");


                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-Api-Key", this.tokenAPI);
                IRestResponse response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {

                    cmbPasta.DataSource = null;
                    listFolders.Clear();

                    LacunaDocuments pastas = JsonConvert.DeserializeObject<LacunaDocuments>(response.Content);



                    foreach (Item pasta in pastas.Items)
                    {
                        Folder folder = new Folder();

                        folder.Id = pasta.Id;
                        folder.Name = pasta.Name;

                        listFolders.Add(folder);
                    }
                    cmbPasta.DataSource = listFolders;
                    cmbPasta.DisplayMember = "Name";
                    cmbPasta.ValueMember = "Id";
                    cmbPasta.SelectedIndex = -1;
                    cmbPasta.Refresh();

                    Application.DoEvents();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void cmbPasta_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                //============== PENDENTES =================
                listPendentes.Clear();
                dgvPendentes.DataSource = null;
                var folderRest = new RestClient(link + "documents?IsConcluded=false&FolderId=" + cmbPasta.SelectedValue + @"&Limit=10000");

                folderRest.Timeout = -1;
                var requestFolder = new RestRequest(Method.GET);
                requestFolder.AddHeader("X-Api-Key", this.tokenAPI);
                IRestResponse responseP = folderRest.Execute(requestFolder);

                if (responseP != null && !string.IsNullOrEmpty(responseP.Content))
                {
                    LacunaDocuments documentos = JsonConvert.DeserializeObject<LacunaDocuments>(responseP.Content);

                    foreach (Item documento in documentos.Items)
                    {
                        Pendentes clsPendentes = new Pendentes();
                        clsPendentes.Id = documento.Id;
                        clsPendentes.Nome = documento.Name;
                        clsPendentes.Data = documento.CreationDate;

                        listPendentes.Add(clsPendentes);

                    }

                    listPendentes = (from o in listPendentes
                                     orderby o.Data descending
                                     select o).ToList();

                    dgvPendentes.DataSource = listPendentes;
                    Application.DoEvents();
                }

                //============== CONCLUÍDOS =================
                listConcluidos.Clear();
                dgvConcluidos.DataSource = null;
                var folderRestConcluidos = new RestClient(link + "documents?IsConcluded=true&FolderId=" + cmbPasta.SelectedValue + @"&Limit=10000");

                folderRestConcluidos.Timeout = -1;
                var requestFolderConcluidos = new RestRequest(Method.GET);
                requestFolderConcluidos.AddHeader("X-Api-Key", this.tokenAPI);
                IRestResponse responseC = folderRestConcluidos.Execute(requestFolderConcluidos);

                if (responseC != null && !string.IsNullOrEmpty(responseC.Content))
                {
                    LacunaDocuments documentosConcluidos = JsonConvert.DeserializeObject<LacunaDocuments>(responseC.Content);


                    foreach (Item documento in documentosConcluidos.Items)
                    {
                        Concluidos clsConcluidos = new Concluidos();

                        clsConcluidos.Id = documento.Id;
                        clsConcluidos.Nome = documento.Name;
                        clsConcluidos.Data = documento.CreationDate;

                        listConcluidos.Add(clsConcluidos);
                    }

                    dgvConcluidos.DataSource = listConcluidos;
                    Application.DoEvents();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
               
        private void EnviarNotificacao(string documenttID, string flowID)
        {
            var client = new RestClient(link + "notifications/flow-action-reminder");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("X-Api-Key", this.tokenAPI);
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

        private void dgvPendentes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LimpaDetalhamentos();
            //Clicou no botão
            if (e.ColumnIndex == 4)
            {
                string id_documento = (string)dgvPendentes.Rows[e.RowIndex].Cells[1].Value;


                string linkPendentes = link + "documents/" + id_documento;

                var clientItem = new RestClient(linkPendentes);
                clientItem.Timeout = -1;
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
                            pnlDetalhes.Visible = true;
                            lblPasta.Text += detalhamentoDoDocumento.Folder.Name.ToString();
                            lblOrg.Text += detalhamentoDoDocumento.Organization.Name.ToString();
                            lblNomeDoc.Text += detalhamentoDoDocumento.Name.ToString();
                            lblEnviadoEm.Text += detalhamentoDoDocumento.CreationDate.ToString();
                            lblUltAtualizacao.Text += detalhamentoDoDocumento.UpdateDate.ToString();
                            lblSubmetido.Text += detalhamentoDoDocumento.CreatedBy.Name.ToString();

                            if (detalhamentoDoDocumento.Status.ToString() == "Pending")
                            {
                                lblStatus.Text += "Pendente";
                            }
                            else
                            {
                                lblStatus.Text += "Concluído";
                            }

                            foreach (FlowAction acao in detalhamentoDoDocumento.FlowActions)
                            {
                                OrdemAcao clsOrdemAcao = new OrdemAcao();

                                clsOrdemAcao.Nome = acao.User.Name.ToString();

                                if (acao.Status == "Pending" || acao.Status == "Created")
                                {
                                    clsOrdemAcao.Status = "Pendente";
                                }
                                else
                                {
                                    clsOrdemAcao.Status = "Assinado";
                                }


                                switch (acao.Type)
                                {
                                    case "Signer":
                                        {
                                            clsOrdemAcao.Acao = "Signatário";
                                        }
                                        break;
                                    case "Approver":
                                        {
                                            clsOrdemAcao.Acao = "Aprovador";
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                listOrdemAcao.Add(clsOrdemAcao);
                            }
                            dgvDetalhes.DataSource = listOrdemAcao;
                            Application.DoEvents();
                        }
                    }
                }
            }
        }

        private void dgvConcluidos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LimpaDetalhamentos();
            //Clicou no botão
            if (e.ColumnIndex == 3)
            {
                string id_documento = (string)dgvConcluidos.Rows[e.RowIndex].Cells[0].Value;


                string linkConcluidos = link + "documents/" + id_documento;

                var clientItem = new RestClient(linkConcluidos);
                clientItem.Timeout = -1;
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
                            pnlDetalhes.Visible = true;
                            lblPasta.Text += detalhamentoDoDocumento.Folder.Name.ToString();
                            lblOrg.Text += detalhamentoDoDocumento.Organization.Name.ToString();
                            lblNomeDoc.Text += detalhamentoDoDocumento.Name.ToString();
                            lblEnviadoEm.Text += detalhamentoDoDocumento.CreationDate.ToString();
                            lblUltAtualizacao.Text += detalhamentoDoDocumento.UpdateDate.ToString();
                            lblSubmetido.Text += detalhamentoDoDocumento.CreatedBy.Name.ToString();

                            if (detalhamentoDoDocumento.Status.ToString() == "Pending")
                            {
                                lblStatus.Text += "Pendente";
                            }
                            else
                            {
                                lblStatus.Text += "Concluído";
                            }

                            foreach (FlowAction acao in detalhamentoDoDocumento.FlowActions)
                            {
                                OrdemAcao clsOrdemAcao = new OrdemAcao();

                                clsOrdemAcao.Nome = acao.User.Name.ToString();
                                if (acao.Status == "Pending")
                                {
                                    clsOrdemAcao.Status = "Pendente";
                                }
                                else
                                {
                                    clsOrdemAcao.Status = "Assinado";
                                }

                                switch (acao.Type)
                                {
                                    case "Signer":
                                        {
                                            clsOrdemAcao.Acao = "Signatário";
                                        }
                                        break;
                                    case "Approver":
                                        {
                                            clsOrdemAcao.Acao = "Aprovador";
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                listOrdemAcao.Add(clsOrdemAcao);
                            }
                            dgvDetalhes.DataSource = listOrdemAcao;
                            Application.DoEvents();
                        }
                    }
                }
            }
        }

        private void LimpaDetalhamentos()
        {
            dgvDetalhes.DataSource = null;
            if (listOrdemAcao != null)
            {
                listOrdemAcao.Clear();
            }
            pnlDetalhes.Visible = false;
            lblOrg.Text = "Organização: ";
            lblPasta.Text = "Pasta: ";
            lblNomeDoc.Text = "Documento: ";
            lblEnviadoEm.Text = "Enviado em: ";
            lblUltAtualizacao.Text = "Última atualização: ";
            lblSubmetido.Text = "Submetido por: ";
            lblStatus.Text = "Status: ";
        }



        private void btnSelDir_Click(object sender, EventArgs e)
        {
            try
            {
                //DialogResult result = folderBrowserDocs.ShowDialog();

                //if (result == DialogResult.OK)
                //{
                //    txtDirDoc.Text = folderBrowserDocs.SelectedPath;
                //}


                OpenFileDialog folderBrowser = new OpenFileDialog();

                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;
                folderBrowser.Filter = "Pdf Files|*.pdf";

                folderBrowser.FileName = "Selecione uma pasta";
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    DesabilitaBtnExecutar();
                    txtDirDoc.Text = Path.GetDirectoryName(folderBrowser.FileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelAss_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = openFileXLSX.ShowDialog();

                if (result == DialogResult.OK)
                {
                    DesabilitaBtnExecutar();
                    txtAssPlanilha.Text = openFileXLSX.FileName;
                }
            }
            catch (Exception ex)
            {
                DesabilitaBtnExecutar();
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<PessoaFluxo> RetornaListaPessasFluxo()
        {
            List<PessoaFluxo> listaPessoaFluxo = new List<PessoaFluxo>();

            if (dataGridView1.Rows != null && dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow linha in dataGridView1.Rows)
                {
                    if (linha.Cells[0].Value != null && !string.IsNullOrEmpty(linha.Cells[0].Value.ToString()) &&
                        linha.Cells[1].Value != null && !string.IsNullOrEmpty(linha.Cells[1].Value.ToString()) &&
                        linha.Cells[2].Value != null && !string.IsNullOrEmpty(linha.Cells[2].Value.ToString()) &&
                        linha.Cells[4].Value != null && !string.IsNullOrEmpty(linha.Cells[4].Value.ToString()) &&
                        linha.Cells[5].Value != null && !string.IsNullOrEmpty(linha.Cells[5].Value.ToString()))
                    {
                        PessoaFluxo pessoaFluxo = new PessoaFluxo();

                        //Tipo
                        if (linha.Cells[0].Value != null && !string.IsNullOrEmpty(linha.Cells[0].Value.ToString()))
                        {
                            if (linha.Cells[0].Value.ToString() == "Signatário")
                            {
                                pessoaFluxo.tipo = FlowActionType.Signer;
                            }
                            else if (linha.Cells[0].Value.ToString() == "Aprovador")
                            {
                                pessoaFluxo.tipo = FlowActionType.Approver;
                            }
                            else if (linha.Cells[0].Value.ToString() == "Observador")
                            {
                                pessoaFluxo.tipo = FlowActionType.SignRule;
                            }
                        }

                        //Ordem
                        if (linha.Cells[1].Value != null && !string.IsNullOrEmpty(linha.Cells[1].Value.ToString()))
                        {
                            pessoaFluxo.ordem = Convert.ToInt32(linha.Cells[1].Value);
                        }

                        //Nome
                        if (linha.Cells[2].Value != null && !string.IsNullOrEmpty(linha.Cells[2].Value.ToString()))
                        {
                            pessoaFluxo.nome = linha.Cells[2].Value.ToString();
                        }

                        //Título
                        pessoaFluxo.titulo = linha.Cells[3].Value.ToString();


                        //CPF
                        if (linha.Cells[4].Value != null && !string.IsNullOrEmpty(linha.Cells[4].Value.ToString()))
                        {
                            pessoaFluxo.cpf = linha.Cells[4].Value.ToString().Replace("-", string.Empty).Replace(".", string.Empty);
                        }

                        //Email
                        if (linha.Cells[5].Value != null && !string.IsNullOrEmpty(linha.Cells[5].Value.ToString()))
                        {
                            pessoaFluxo.email = linha.Cells[5].Value.ToString();
                        }
                        //Telefone
                        if (linha.Cells[6].Value != null && !string.IsNullOrEmpty(linha.Cells[6].Value.ToString()))
                        {
                            pessoaFluxo.telefone = linha.Cells[6].Value.ToString();
                        }

                        //Permitir Ass Eletronica
                        pessoaFluxo.permitirAssEletronica = Convert.ToBoolean(linha.Cells[7].Value);


                        //Adiciona na litsa
                        listaPessoaFluxo.Add(pessoaFluxo);
                    }
                }
            }

            return listaPessoaFluxo;
        }



        //===============Aqui é de organização envio======
        private void cmbOrgEnvio_SelectedIndexChanged(object sender, EventArgs e)
        {
            DesabilitaBtnExecutar();
            this.tokenAPI = "";
            try
            {
                foreach (Organizacao org in this.jsonConfig.organizacoes)
                {
                    // org.
                    if (org.NOME == cmbOrgEnvio.Text)
                    {
                        this.tokenAPI = org.CHAVE;
                        break;
                    }
                }

                var client = new RestClient(link + "folders?Limit=100&Order=Asc");

                client.Timeout = -1;
                var request = new RestRequest(Method.GET);



                //-----------------------------------------------------------------------

               // PaginatedSearchParams quant = new PaginatedSearchParams();
              //  quant.Limit = 100;


               // SignerClient signerClient = new SignerClient(this.link.Replace("api/", string.Empty));

              //  PaginatedSearchResponse<FolderInfoModel> rep =  signerClient.ListFoldersPaginatedAsync(quant, null);

                //-----------------------------------------------------------------------



                request.AddHeader("X-Api-Key", this.tokenAPI);

                IRestResponse response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    listFolders.Clear();

                    LacunaDocuments pastas = JsonConvert.DeserializeObject<LacunaDocuments>(response.Content);

                    if (pastas.Items != null)
                    {
                        foreach (Item pasta in pastas.Items)
                        {
                            Folder folder = new Folder();

                            folder.Id = pasta.Id;
                            folder.Name = pasta.Name;

                            listFolders.Add(folder);
                            
                           TreeNode rootNode = treeView1.Nodes.Add(folder.Name);
                           TreeNode child = rootNode.Nodes.Add("");
                            

                        }
                        
                        cmbPastaEnvio.DataSource = listFolders;
                        cmbPastaEnvio.ValueMember = "Id";
                        cmbPastaEnvio.DisplayMember = "Name";
                        cmbPastaEnvio.SelectedIndex = -1;
                        cmbPastaEnvio.Refresh();

                    }
                        
                   
                        Application.DoEvents();

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnNotificar_Click(object sender, EventArgs e)
        {
            try
            {
                bool enviado = false;
                this.prgDocumento.Value = 0;
                if (tabControlDocumentos.SelectedTab == tabPagePendentes)
                {
                    foreach (DataGridViewRow dr in dgvPendentes.Rows)
                    {
                        this.prgDocumento.Maximum = dgvPendentes.Rows.Count;
                        this.prgDocumento.Visible = true;
                        if (dr.Cells[0].Value != null)
                        {
                            string linkNotificacao = link + "documents/" + dr.Cells[1].Value;

                            var clientItem = new RestClient(linkNotificacao);
                            clientItem.Timeout = -1;
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

                                        Application.DoEvents();

                                        foreach (FlowAction acao in detalhamentoDoDocumento.FlowActions)
                                        {
                                            EnviarNotificacao(dr.Cells[1].Value.ToString(), acao.Id);
                                            enviado = true;
                                        }
                                    }
                                    else
                                    {
                                        enviado = false;
                                    }
                                }
                            }
                        }
                        this.prgDocumento.Value += 1;
                        Application.DoEvents();
                    }

                    if (enviado)
                    {
                        MessageBox.Show("Noficações enviadas com sucesso!");
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível enviar as notificações");
                    }
                }
                else
                {
                    MessageBox.Show("É necessário selecionar ao menos um documento na guia de pendentes");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.prgDocumento.Visible = false;
                Application.UseWaitCursor = false;
                Application.DoEvents();
            }
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            DesabilitaBtnExecutar();
            if (cmbOrgEnvio.Text.Length > 0 && cmbPastaEnvio.Text.Length > 0)
            {
                totalLinhas = 0;

                try
                {
                    this.Enabled = false;
                    Application.UseWaitCursor = true;

                    this.btnExecutar.Enabled = false;

                    this.tabSucesso.Text = "Log Sucesso";
                    this.tabErro.Text = "Log Erro";

                    this.txtLog.Text = string.Empty;
                    this.txtLogERRO.Text = string.Empty;

                    string diretorio = this.txtDirDoc.Text;
                    string arquivoExcel = this.txtAssPlanilha.Text;

                    int totalOK = 0;
                    int totalErro = 0;


                    this.progressBarVal.Value = 0;
                    Application.DoEvents();

                    if (!string.IsNullOrEmpty(diretorio) && Directory.Exists(diretorio))
                    {
                        if (File.Exists(arquivoExcel))
                        {
                            string[] Arquivos = Directory.GetFiles(diretorio);


                            if (Arquivos != null && Arquivos.Length > 0)
                            {
                                this.progressBarVal.Maximum = Arquivos.Length;
                                this.progressBarVal.Visible = true;


                                XLWorkbook xls = new XLWorkbook(arquivoExcel);
                                IXLWorksheet planilha = xls.Worksheets.First();
                                totalLinhas = planilha.Rows().Count();

                                // int linha = 3;
                                foreach (string itemArquivo in Arquivos)
                                {
                                    Application.DoEvents();
                                    string ext = Path.GetExtension(itemArquivo);

                                    if (ext.ToLower() == ".pdf")
                                    {

                                        bool achou = false;

                                        string nomeArquivo = itemArquivo.Replace(diretorio, string.Empty).Replace(@"\", string.Empty);

                                        int incrementoIndice = 1;

                                        // List<PessoaFluxo> listaPessoasFluxo = this.RetornaListaPessasFluxo();


                                        // primeira linha é o cabecalho
                                        for (int l = 3; l <= totalLinhas; l++)
                                        {
                                            string nome = planilha.Cell(l, 1).Value.ToString();

                                            //string emailNotificarConclusao = planilha.Cell(l, 2).Value.ToString();
                                            //List<string> listaNotifiedEmails = new List<string>();
                                            //if (!(string.IsNullOrEmpty(emailNotificarConclusao) || string.IsNullOrEmpty(emailNotificarConclusao.Trim())))
                                            //{
                                            //    string[] subs = emailNotificarConclusao.Split(',');
                                            //    foreach (var sub in subs)
                                            //    {
                                            //        listaNotifiedEmails.Add(sub);
                                            //    }
                                            //}


                                            // if (nomeArquivo.ToUpper().Contains(nome.ToUpper()) && !string.IsNullOrEmpty(nome))
                                            if (!string.IsNullOrEmpty(nome) && nomeArquivo.ToUpper().Replace(".PDF", "") == nome.ToUpper().Replace(".PDF", ""))
                                            {
                                                #region Pessoa Fluxo
                                                string proximaPessoa = planilha.Cell(l, 2 + incrementoIndice).Value.ToString();

                                                while (!string.IsNullOrEmpty(proximaPessoa) == true)
                                                {
                                                    achou = true;

                                                    //c
                                                    string cpf = planilha.Cell(l, 3 + incrementoIndice).Value.ToString().Trim().Replace("-", string.Empty).Replace(".", string.Empty);

                                                    //D
                                                    string email = planilha.Cell(l, 4 + incrementoIndice).Value.ToString();

                                                    //E
                                                    string tel = planilha.Cell(l, 5 + incrementoIndice).Value.ToString();

                                                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()) || emailIsValid(email) == false)
                                                    {
                                                        totalErro++;
                                                        this.txtLogERRO.Text += $"{nomeArquivo} - Participante: {proximaPessoa} - Email ({email}) inválido" + Environment.NewLine;
                                                        //break;
                                                    }
                                                    else if (string.IsNullOrEmpty(cpf) || cpf.Length < 11 || IsCpf(cpf) == false)
                                                    {
                                                        totalErro++;
                                                        this.txtLogERRO.Text += $"{nomeArquivo} - Participante: {proximaPessoa} - CPF {cpf} Inválido" + Environment.NewLine;
                                                        // break;
                                                    }
                                                    else if (
                                                          (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() != "SIGNATÁRIO") &&
                                                          (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() != "SIGNATARIO") &&
                                                          (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() != "APROVADOR") &&
                                                          (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() != "OBSERVADOR")
                                                          )
                                                    {
                                                        totalErro++;
                                                        this.txtLogERRO.Text += $"{nomeArquivo} - Participante: {proximaPessoa} - ERRO encontrado na coluna tipo. Campos permitidos: SIGNATÁRIO, APROVADOR, OBSERVADOR" + Environment.NewLine;
                                                        // break;
                                                    }
                                                    else
                                                    {
                                                        totalOK++;
                                                        this.txtLog.Text += $"{nomeArquivo} - Participante: {proximaPessoa} - OK" + Environment.NewLine;
                                                        // break;
                                                    }
                                                    incrementoIndice = incrementoIndice + 8;

                                                    proximaPessoa = planilha.Cell(l, 2 + incrementoIndice).Value.ToString();
                                                    #endregion Pessoa Fluxo
                                                }

                                                Application.DoEvents();
                                            }
                                        }


                                        this.progressBarVal.Value += 1;
                                        Application.DoEvents();
                                    }
                                } // Fim Foreach

                                this.tabSucesso.Text = "Log Sucesso (" + totalOK.ToString() + ")";
                                this.tabErro.Text = "Log Erro (" + totalErro.ToString() + ")";

                                if (totalErro == 0)
                                {
                                    this.btnExecutar.Visible = true;
                                    this.btnExecutar.Enabled = true;
                                }


                                Application.DoEvents();
                            }
                            else
                            {
                                MessageBox.Show("Arquivos não encontrados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.btnExecutar.Visible = false;

                            }
                        }
                        else
                        {
                            this.btnExecutar.Visible = false;
                            MessageBox.Show("Arquivos com o fluxo de assinaturas não encontrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.btnExecutar.Visible = false;
                        MessageBox.Show("Diretório inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    this.btnExecutar.Visible = false;
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Enabled = true;
                    this.progressBarVal.Visible = false;
                    Application.UseWaitCursor = false;
                    Application.DoEvents();
                }

            }



            //if (cmbOrgEnvio.Text.Length > 0 && cmbPastaEnvio.Text.Length > 0)
            //{
            //    totalLinhas = 0;
            //    try
            //    {
            //        this.Enabled = false;
            //        Application.UseWaitCursor = true;

            //        this.btnExecutar.Enabled = false;

            //        this.tabSucesso.Text = "Log Sucesso";
            //        this.tabErro.Text = "Log Erro";

            //        this.txtLog.Text = string.Empty;
            //        this.txtLogERRO.Text = string.Empty;

            //        string diretorio = this.txtDirDoc.Text;
            //        string arquivoExcel = this.txtAssPlanilha.Text;

            //        int totalOK = 0;
            //        int totalErro = 0;

            //        this.progressBarVal.Value = 0;



            //        if (!string.IsNullOrEmpty(diretorio) && Directory.Exists(diretorio))
            //        {
            //            if (File.Exists(arquivoExcel))
            //            {
            //                string[] Arquivos = Directory.GetFiles(diretorio);

            //                if (Arquivos != null && Arquivos.Length > 0)
            //                {
            //                    this.progressBarVal.Maximum = Arquivos.Length;
            //                    this.progressBarVal.Visible = true;


            //                    XLWorkbook xls = new XLWorkbook(arquivoExcel);
            //                    IXLWorksheet planilha = xls.Worksheets.First();
            //                    totalLinhas = planilha.Rows().Count();

            //                    foreach (string itemArquivo in Arquivos)
            //                    {
            //                        Application.DoEvents();
            //                        string ext = Path.GetExtension(itemArquivo);

            //                        if (ext.ToLower() == ".pdf")
            //                        {

            //                            //Lendo Excel -------------------------------------------------
            //                            bool achou = false;

            //                            string nomeArquivo = itemArquivo.Replace(diretorio, string.Empty).Replace(@"\", string.Empty);


            //                            // primeira linha é o cabecalho
            //                            for (int l = 3; l <= totalLinhas; l++)
            //                            {
            //                                string nome = planilha.Cell($"A{l}").Value.ToString();
            //                                string envioEmailConcluido = planilha.Cell($"B{l}").Value.ToString();
            //                                // if (nomeArquivo.ToUpper().Contains(nome.ToUpper()) && !string.IsNullOrEmpty(nome))
            //                                if (!string.IsNullOrEmpty(nome) && nomeArquivo.ToUpper().Replace(".PDF", "") == nome.ToUpper().Replace(".PDF", ""))
            //                                {
            //                                    achou = true;

            //                                    string nomeCompleto = planilha.Cell($"C{l}").Value.ToString();
            //                                    string cpf = planilha.Cell($"D{l}").Value.ToString();
            //                                    string email = planilha.Cell($"E{l}").Value.ToString();
            //                                    string tel = planilha.Cell($"F{l}").Value.ToString();
            //                                    string tipo = planilha.Cell($"G{l}").Value.ToString();
            //                                    string ordem = planilha.Cell($"H{l}").Value.ToString();
            //                                    string permitirAssElet = planilha.Cell($"I{l}").Value.ToString();
            //                                    string titulo = planilha.Cell($"J{l}").Value.ToString();

            //                                    if (cpf.Length == 11 && !string.IsNullOrEmpty(cpf))
            //                                    {
            //                                        cpf = cpf.Trim().Replace(".", string.Empty).Replace("-", string.Empty);
            //                                    }
            //                                    else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - E-mail inválido" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    else if (string.IsNullOrEmpty(cpf) || cpf.Length < 11)
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - ERRO - (CPF {cpf} Inválido)" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    else if (string.IsNullOrEmpty(tel))
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - Telefone Inválido" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    else if (string.IsNullOrEmpty(tipo))
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - Tipo Inválido" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    else if (string.IsNullOrEmpty(ordem))
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - Ordem Inválida" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    else if (string.IsNullOrEmpty(permitirAssElet))
            //                                    {
            //                                        totalErro++;
            //                                        this.txtLogERRO.Text += $"{nomeArquivo} - Permissão inválida" + Environment.NewLine;
            //                                        achou = false;
            //                                    }
            //                                    //else
            //                                    //{
            //                                    //    totalErro++;
            //                                    //    this.txtLogERRO.Text += $"{nomeArquivo} - CPF inválido" + Environment.NewLine;
            //                                    //    achou = false;
            //                                    //}

            //                                    if (achou)
            //                                    {
            //                                        totalOK++;

            //                                        string linha = $"{nomeArquivo} - Encontrou participante" + Environment.NewLine;

            //                                        this.txtLog.Text += linha;
            //                                    }
            //                                    else if (!achou)
            //                                    {
            //                                        totalErro++;
            //                                  //      this.txtLogERRO.Text += $"{nomeArquivo} - NÃO Encontrou participante" + Environment.NewLine;
            //                                    }
            //                                    Application.DoEvents();

            //                                    // break;
            //                                }
            //                            }

            //                            //if (!achou)
            //                            //{
            //                            //    totalErro++;
            //                            //    this.txtLogERRO.Text += $"{nomeArquivo} - NÃO Encontrou participante" + Environment.NewLine;
            //                            //}

            //                            this.progressBarVal.Value += 1;
            //                            Application.DoEvents();

            //                        }
            //                        //-------------------------------------------------------------
            //                    }

            //                    this.tabSucesso.Text = "Log Sucesso (" + totalOK.ToString() + ")";
            //                    this.tabErro.Text = "Log Erro (" + totalErro.ToString() + ")";

            //                    if (totalOK > 0)
            //                    {
            //                        this.btnExecutar.Enabled = true;
            //                    }
            //                    else
            //                    {
            //                        this.btnExecutar.Enabled = false;
            //                    }

            //                    Application.DoEvents();

            //                    //MessageBox.Show("Arquivos renomeados com sucesso.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                }
            //                else
            //                {
            //                    MessageBox.Show("Arquivos não encontrados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                }
            //            }
            //            else
            //            {
            //                MessageBox.Show("Arquivos com o fluxo de assinaturas não encontrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Diretório inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        this.Enabled = true;
            //        Application.UseWaitCursor = false;
            //        this.progressBarVal.Visible = false;
            //        Application.DoEvents();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("É necessário selecionar a organização e a pasta");
            //}
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            totalLinhas = 0;

            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;

                this.btnExecutar.Enabled = false;

                this.tabSucesso.Text = "Log Sucesso";
                this.tabErro.Text = "Log Erro";

                this.txtLog.Text = string.Empty;
                this.txtLogERRO.Text = string.Empty;

                string diretorio = this.txtDirDoc.Text;
                string arquivoExcel = this.txtAssPlanilha.Text;

                int totalOK = 0;
                int totalErro = 0;
                bool corretoParaEnvio = false;


                this.progressBarVal.Value = 0;
                Application.DoEvents();

                if (!string.IsNullOrEmpty(diretorio) && Directory.Exists(diretorio))
                {
                    if (File.Exists(arquivoExcel))
                    {
                        string[] Arquivos = Directory.GetFiles(diretorio);


                        if (Arquivos != null && Arquivos.Length > 0)
                        {
                            this.progressBarVal.Maximum = Arquivos.Length;
                            this.progressBarVal.Visible = true;


                            XLWorkbook xls = new XLWorkbook(arquivoExcel);
                            IXLWorksheet planilha = xls.Worksheets.First();
                            totalLinhas = planilha.Rows().Count();

                            // int linha = 3;
                            foreach (string itemArquivo in Arquivos)
                            {
                                Application.DoEvents();
                                string ext = Path.GetExtension(itemArquivo);

                                if (ext.ToLower() == ".pdf")
                                {

                                    bool achou = false;

                                    string nomeArquivo = itemArquivo.Replace(diretorio, string.Empty).Replace(@"\", string.Empty);

                                    int incrementoIndice = 1;

                                    List<PessoaFluxo> listaPessoasFluxo = this.RetornaListaPessasFluxo();


                                    // primeira linha é o cabecalho
                                    for (int l = 3; l <= totalLinhas; l++)
                                    {
                                        string nome = planilha.Cell(l, 1).Value.ToString();

                                        string emailNotificarConclusao = planilha.Cell(l, 2).Value.ToString();
                                        List<string> listaNotifiedEmails = new List<string>();
                                        if (!(string.IsNullOrEmpty(emailNotificarConclusao) || string.IsNullOrEmpty(emailNotificarConclusao.Trim())))
                                        {
                                            string[] subs = emailNotificarConclusao.Split(',');
                                            foreach (var sub in subs)
                                            {
                                                listaNotifiedEmails.Add(sub);
                                            }
                                        }


                                        // if (nomeArquivo.ToUpper().Contains(nome.ToUpper()) && !string.IsNullOrEmpty(nome))
                                        if (!string.IsNullOrEmpty(nome) && nomeArquivo.ToUpper().Replace(".PDF", "") == nome.ToUpper().Replace(".PDF", ""))
                                        {
                                            #region Pessoa Fluxo
                                            string proximaPessoa = planilha.Cell(l, 2 + incrementoIndice).Value.ToString();

                                            while (!string.IsNullOrEmpty(proximaPessoa) == true)
                                            {
                                                achou = true;

                                                //c
                                                string cpf = planilha.Cell(l, 3 + incrementoIndice).Value.ToString().Trim().Replace("-", string.Empty).Replace(".", string.Empty);

                                                //D
                                                string email = planilha.Cell(l, 4 + incrementoIndice).Value.ToString();

                                                //E
                                                string tel = planilha.Cell(l, 5 + incrementoIndice).Value.ToString();

                                                if ((string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim())) && (string.IsNullOrEmpty(cpf) || cpf.Length < 11))
                                                {
                                                    totalErro++;
                                                    corretoParaEnvio = false;
                                                    this.txtLogERRO.Text += $"{nomeArquivo} - CPF e E-mail inválidos" + Environment.NewLine;
                                                    break;
                                                }
                                                else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
                                                {
                                                    totalErro++;
                                                    corretoParaEnvio = false;
                                                    this.txtLogERRO.Text += $"{nomeArquivo} - E-mail inválido" + Environment.NewLine;
                                                    break;
                                                }
                                                else if (string.IsNullOrEmpty(cpf) || cpf.Length < 11)
                                                {
                                                    totalErro++;
                                                    corretoParaEnvio = false;
                                                    this.txtLogERRO.Text += $"{nomeArquivo} - CPF Inválido" + Environment.NewLine;
                                                    break;
                                                }
                                                //else if (string.IsNullOrEmpty(tel) || tel.Length < 11)
                                                //{
                                                //    totalErro++;
                                                //    this.txtLogERRO.Text += $"{nomeArquivo} - Telefone Inválido" + Environment.NewLine;
                                                //}
                                                else
                                                {
                                                    totalOK++;
                                                    corretoParaEnvio = true;

                                                    if (listaPessoasFluxo == null)
                                                        listaPessoasFluxo = new List<PessoaFluxo>();

                                                    PessoaFluxo pessoaCSV = new PessoaFluxo();

                                                    //B
                                                    pessoaCSV.nome = planilha.Cell(l, 2 + incrementoIndice).Value.ToString();

                                                    //C
                                                    pessoaCSV.cpf = planilha.Cell(l, 3 + incrementoIndice).Value.ToString().Replace("-", string.Empty).Replace(".", string.Empty);

                                                    //D
                                                    pessoaCSV.email = planilha.Cell(l, 4 + incrementoIndice).Value.ToString();

                                                    //E
                                                    pessoaCSV.telefone = planilha.Cell(l, 5 + incrementoIndice).Value.ToString();

                                                    //F
                                                    if ((planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() == "SIGNATÁRIO") ||
                                                        (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() == "SIGNATARIO") ||
                                                        (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() == "APROVADOR") ||
                                                        (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper() == "OBSERVADOR"))
                                                    {
                                                        switch (planilha.Cell(l, 6 + incrementoIndice).Value.ToString().ToUpper())
                                                        {
                                                            case "SIGNATÁRIO":
                                                                {
                                                                    pessoaCSV.tipo = FlowActionType.Signer;
                                                                }
                                                                break;
                                                            case "SIGNATARIO":
                                                                {
                                                                    pessoaCSV.tipo = FlowActionType.Signer;
                                                                }
                                                                break;
                                                            case "APROVADOR":
                                                                {
                                                                    pessoaCSV.tipo = FlowActionType.Approver;
                                                                }
                                                                break;
                                                            case "OBSERVADOR":
                                                                {
                                                                    pessoaCSV.tipo = FlowActionType.SignRule;
                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        totalErro++;
                                                        this.txtLogERRO.Text += $"{nomeArquivo} - Participante: {proximaPessoa} - ERRO encontrado na coluna tipo. Campos permitidos: SIGNATÁRIO, APROVADOR, OBSERVADOR" + Environment.NewLine;

                                                        break;
                                                    }

                                                    //G
                                                    pessoaCSV.ordem = Convert.ToInt32(planilha.Cell(l, 7 + incrementoIndice).Value);

                                                    //H
                                                    if (planilha.Cell(l, 8 + incrementoIndice).Value.ToString().ToUpper() == "NÃO" || planilha.Cell(l, 8 + incrementoIndice).Value.ToString().ToUpper() == "NAO")
                                                    {
                                                        pessoaCSV.permitirAssEletronica = false;
                                                    }
                                                    else if (planilha.Cell(l, 8 + incrementoIndice).Value.ToString().ToUpper() == "SIM")
                                                    {
                                                        pessoaCSV.permitirAssEletronica = true;
                                                    }

                                                    pessoaCSV.titulo = planilha.Cell(l, 9 + incrementoIndice).Value.ToString();


                                                    listaPessoasFluxo.Add(pessoaCSV);

                                                    incrementoIndice = incrementoIndice + 8;

                                                    proximaPessoa = planilha.Cell(l, 2 + incrementoIndice).Value.ToString();
                                                }
                                                #endregion Pessoa Fluxo
                                            }

                                            if (corretoParaEnvio)
                                            {
                                                List<PessoaFluxo> listaPessoasFluxoOrdenada = new List<PessoaFluxo>();

                                                listaPessoasFluxoOrdenada = listaPessoasFluxo.OrderBy(x => x.ordem).ToList();

                                                LacunaEnviarDocComOrdemDeAssinaturas objEnviaDoc = new LacunaEnviarDocComOrdemDeAssinaturas(this.link.Replace("api/", string.Empty), this.tokenAPI, this.txtLog, this.txtLogERRO, cmbPastaEnvio.SelectedValue.ToString(), posAssinatura.Checked);

                                                objEnviaDoc.EnviaDoc(itemArquivo, listaPessoasFluxoOrdenada, listaNotifiedEmails);

                                                Thread.Sleep(1000);

                                                corretoParaEnvio = false;
                                            }

                                            Application.DoEvents();

                                            break;
                                        }
                                    }

                                    if (!achou)
                                    {
                                        totalErro++;
                                        this.txtLogERRO.Text += $"{nomeArquivo} - NÃO Encontrou assinador" + Environment.NewLine;
                                    }

                                    this.progressBarVal.Value += 1;
                                    Application.DoEvents();
                                    //linha += 1;
                                }
                            }

                            this.tabSucesso.Text = "Log Sucesso (" + totalOK.ToString() + ")";
                            this.tabErro.Text = "Log Erro (" + totalErro.ToString() + ")";

                            if (totalOK > 0)
                            {
                                this.btnExecutar.Enabled = true;
                            }
                            else
                            {
                                this.btnExecutar.Enabled = false;
                            }

                            Application.DoEvents();

                            //MessageBox.Show("Arquivos renomeados com sucesso.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Arquivos não encontrados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Arquivos com o fluxo de assinaturas não encontrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Diretório inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DesabilitaBtnExecutar();
                this.progressBarVal.Visible = false;
                this.Enabled = true;
                Application.UseWaitCursor = false;
                Application.DoEvents();
            }
        }

        private void btnSalvarJsonOrg_Click(object sender, EventArgs e)
        {
            const string message = "Você deseja salvar essas organizações?";
            const string caption = "Atenção!";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.Yes)
            {
                if (dgvOrganizacoes.Rows != null && dgvOrganizacoes.Rows.Count > 0)
                {
                    ListOrgConfig.Clear();
                    foreach (DataGridViewRow linha in dgvOrganizacoes.Rows)
                    {
                        if (linha.Cells[1].Value != null && !string.IsNullOrEmpty(linha.Cells[1].Value.ToString()) &&
                            linha.Cells[2].Value != null && !string.IsNullOrEmpty(linha.Cells[2].Value.ToString()))
                        {
                            Organizacao clsOrgConfig = new Organizacao();

                            clsOrgConfig.NOME = linha.Cells[1].Value.ToString();
                            clsOrgConfig.CHAVE = linha.Cells[2].Value.ToString();

                            ListOrgConfig.Add(clsOrgConfig);
                        }
                    }

                    jsonConfig.organizacoes = ListOrgConfig;

                    string JSON_FINAL = JsonConvert.SerializeObject(jsonConfig);

                    File.WriteAllText(Application.StartupPath + @"\config.json", JSON_FINAL);

                    dgvOrganizacoes.Rows.Clear();

                    LimpaCombox();
                    this.LerJson();

                    if (dgvOrganizacoes.RowCount <= 1)
                    {
                        foreach (var org in jsonConfig.organizacoes)
                        {
                            dgvOrganizacoes.Rows.Add(false, org.NOME.ToString(), org.CHAVE.ToString());
                        }
                    }
                    //Esconde_tabPages();

                    MessageBox.Show("Salvo com sucesso!\nAntes de utilizar as novas organizações, é necessário reiniciar o sistema");
                }
            }
        }

        private void btnExcluirorg_Click(object sender, EventArgs e)
        {
            const string message = "Você deseja excluir todas as organizações selecionadas?";
            const string caption = "Atenção!";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.Yes)
            {
                try
                {
                    ListOrgConfig.Clear();
                    foreach (DataGridViewRow dr in dgvOrganizacoes.Rows)
                    {
                        if (dr.Cells[1].Value != null && !string.IsNullOrEmpty(dr.Cells[1].Value.ToString()) &&
                            dr.Cells[2].Value != null && !string.IsNullOrEmpty(dr.Cells[2].Value.ToString()))
                        {
                            if (Convert.ToBoolean(dr.Cells[0].Value) == false)
                            {
                                Organizacao clsOrgConfig = new Organizacao();

                                clsOrgConfig.NOME = dr.Cells[1].Value.ToString();
                                clsOrgConfig.CHAVE = dr.Cells[2].Value.ToString();

                                ListOrgConfig.Add(clsOrgConfig);
                            }
                        }
                    }

                    jsonConfig.organizacoes = ListOrgConfig;

                    string JSON_FINAL = JsonConvert.SerializeObject(jsonConfig);

                    File.WriteAllText(Application.StartupPath + @"\config.json", JSON_FINAL);

                    // dgvOrganizacoes.DataSource = null;
                    dgvOrganizacoes.Rows.Clear();
                    // dgvOrganizacoes.Refresh();

                    LimpaCombox();
                    this.LerJson();


                    if (dgvOrganizacoes.RowCount <= 1)
                    {
                        foreach (var org in jsonConfig.organizacoes)
                        {
                            dgvOrganizacoes.Rows.Add(false, org.NOME.ToString(), org.CHAVE.ToString());
                        }
                    }

                    // Esconde_tabPages();

                    MessageBox.Show("Excluído com sucesso!\nÉ necessário reiniciar o sistema");

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void posAssinatura_CheckedChanged(object sender, EventArgs e)
        {
            if (posAssinatura.Checked == true)
            {

            }
        }

        private void LimpaCombox()
        {
            cmbOrganizacao.DataSource = null;
            cmbOrgEnvio.DataSource = null;
            cmbPasta.DataSource = null;
            cmbPastaEnvio.DataSource = null;
        }

        private void btnAtualizarDocumentos_Click(object sender, EventArgs e)
        {

        }

        private void DesabilitaBtnExecutar()
        {
            this.btnExecutar.Visible = false;
            this.btnExecutar.Enabled = false;
        }

        private void panelCabecalho_Paint(object sender, PaintEventArgs e)
        {

        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }


        public static bool emailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeView1_Click(object sender, EventArgs e)
        {

        }

        private void mostrarpasta(object sender, TreeViewEventArgs e)
        {
            
        }
    }


}
