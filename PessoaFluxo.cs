using Lacuna.Signer.Api;

namespace CERTEDUC.EnvioLote
{
    public class PessoaFluxo
    {
        public FlowActionType tipo { get; set; }

        public int ordem { get; set; }

        public string nome { get; set; }

        public string cpf { get; set; }

        public string email { get; set; }

        public string telefone { get; set; }

        public bool permitirAssEletronica { get; set; }
        public string titulo { get; set; }
    }
}
