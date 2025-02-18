using System.ComponentModel;

namespace OpenAC.Net.NFSe.Providers;

internal enum ISSMapCode
{
    [Description("Processo concluído com sucesso")]
    Sucesso = 100,
    [Description("Erro no recebimento dos dados")]
    ErroRecebimentoDados = 200,
    [Description("Código da chave de criptografia (campo Key) Inválida")]
    CodigoChaveCriptografiaInvalida = 201,
    [Description("Erro de criptografia")]
    ErroCriptografia = 202,
    [Description("Cadastro Contribuinte Mobiliário (CCM) não existe")]
    CadastroContribuinteMobiliarioNaoExiste = 203,
    [Description("CNPJ/CPF do prestador não é a mesma do CCM dono da chave")]
    CnpjCpfPrestadorDiferenteCCMDonoChave = 204,
    [Description("Senha Incorreta ou Inválida")]
    SenhaIncorretaOuInvalida = 205,
    [Description("RPS Inválida (Não encontrou a RPS)")]
    RpsInvalidaNaoEncontrouRps = 206,
    [Description("RPS foi cancelada")]
    RpsFoiCancelada = 207,
    [Description("RPS possui uma solicitação de cancelamento")]
    RpsPossuiSolicitacaoCancelamento = 208,
    [Description("O prazo para cancelamento da nota expirou")]
    PrazoCancelamentoNotaExpirado = 209,
    [Description("Bloqueio de Competencia - Notas em aberto para geração de guia de ISSQN")]
    BloqueioCompetenciaNotasEmAbertoParaGeracaoGuiaISSQN = 210,
    [Description("Contribuinte não liberado para emitir de RPS")]
    ContribuinteNaoLiberadoParaEmitirRPS = 212,
    [Description("Contribuinte está bloqueado no sistema IssMap")]
    ContribuinteBloqueadoNoSistemaIssMap = 213,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco214 = 214,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco215 = 215,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco216 = 216,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco217 = 217,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco218 = 218,
    [Description("Erro no acesso aos Dados do Banco")]
    ErroAcessoDadosBanco219 = 219,
    [Description("Reservados para Dados da Nota Fiscal")]
    ReservadosParaDadosNotaFiscal = 220,
    [Description("Erro na validação do campo Número da RPS")]
    ErroValidacaoCampoNumeroRPS = 221,
    [Description("Erro na validação do campo Data/Hora de Emissão da Nota")]
    ErroValidacaoCampoDataHoraEmissaoNota = 222,
    [Description("Erro na validação do campo Descrição do Serviço da Nota")]
    ErroValidacaoCampoDescricaoNota = 223,
    [Description("Erro na validação do campo Flag de Cancelamento da Nota")]
    ErroValidacaoCampoFlagCancelamentoNota = 224,
    [Description("Erro na validação do campo Motivo do Cancelamento da Nota")]
    ErroValidacaoCampoMotivoCancelamentoNota = 225,
    [Description("Erro na validação do campo Flag Retido na Fonte")]
    ErroValidacaoCampoFlagRetidoNaFonte = 226,
    [Description("Erro na validação do campo Local de Execução")]
    ErroValidacaoCampoLocalExecucao = 227,
    [Description("Erro na validação do campo Código do Serviço Prestado")]
    ErroValidacaoCampoCodigoServicoPrestado = 228,
    [Description("Erro na validação do campo Serviço Prestado")]
    ErroValidacaoCampoServicoPrestado = 229,
    [Description("Erro na validação do campo Valor da Nota")]
    ErroValidacaoCampoValorNota = 230,
    [Description("Erro na validação do campo Valor das Deduções")]
    ErroValidacaoCampoValorDeducoes = 231,
    [Description("Erro na validação do campo Valor da Base de Cálculo")]
    ErroValidacaoCampoValorBaseCalculo = 232,
    [Description("Erro na validação do campo Alíquota")]
    ErroValidacaoCampoAliquota = 233,
    [Description("Erro na validação do campo Valor do ISS")]
    ErroValidacaoCampoValorISS = 234,
    [Description("Retenção não permitida")]
    RetencaoNaoPermitida = 235,
    [Description("Retenção não permitida para pessoa física")]
    RetencaoNaoPermitidaParaPessoaFisica = 236,
    [Description("E-mail Tomador Obrigatório em retenções")]
    EmailTomadorObrigatorioEmRetencoes = 237,
    [Description("Reservados para Dados do Prestador")]
    ReservadoParaDadosPrestador = 240,
    [Description("Erro na validação do campo CPF/CNPJ do Prestador")]
    ErroValidacaoCampoCpfCnpjPrestador = 241,
    [Description("Erro na validação do campo Nome/Razão do Prestador")]
    ErroValidacaoCampoNomePrestador = 242,
    [Description("Erro na validação do campo Endereço do Prestador")]
    ErroValidacaoCampoEnderecoPrestador = 243,
    [Description("Erro na validação do campo Cidade do Prestador")]
    ErroValidacaoCampoCidadePrestador = 244,
    [Description("Erro na validação do campo de Inscrição Municipal do Prestador")]
    ErroValidacaoCampoInscricaoMunicipalPrestador = 245,
    [Description("Erro na validação do campo Inscrição Estadual/Registro Geral do Prestador")]
    ErroValidacaoCampoInscricaoEstadualRGPrestador = 246,
    [Description("Erro na validação do campo Estado do Prestador")]
    ErroValidacaoCampoEstadoPrestador = 247,
    [Description("Erro na validação do campo CEP do Prestador")]
    ErroValidacaoCampoCepPrestador = 248,
    [Description("Erro na validação do campo Email do Prestador")]
    ErroValidacaoCampoEmailPrestador = 249,
    [Description("Reservados para Dados do Tomador")]
    ReservadoParaDadosTomador = 250,
    [Description("Erro na validação do campo CPF/CNPJ do Tomador")]
    ErroValidacaoCampoCpfCnpjTomador = 251,
    [Description("Erro na validação do campo Nome/Razão do Tomador")]
    ErroValidacaoCampoNomeTomador = 252,
    [Description("Erro na validação do campo Endereço do Tomador")]
    ErroValidacaoCampoEnderecoTomador = 253,
    [Description("Erro na validação do campo Cidade do Tomador")]
    ErroValidacaoCampoCidadeTomador = 254,
    [Description("Erro na validação do campo de Inscrição Municipal do Tomador")]
    ErroValidacaoCampoInscricaoMunicipalTomador = 255,
    [Description("Erro na validação do campo Inscrição Estadual/Registro Geral do Tomador")]
    ErroValidacaoCampoInscricaoEstadualRGTomador = 256,
    [Description("Erro na validação do campo Estado do Tomador")]
    ErroValidacaoCampoEstadoTomador = 257,
    [Description("Erro na validação do campo CEP do Tomador")]
    ErroValidacaoCampoCepTomador = 258,
    [Description("Erro na validação do campo Email do Tomador")]
    ErroValidacaoCampoEmailTomador = 259,
    [Description("Reservado para Dados de Autenticação")]
    ReservadoParaDadosAutenticacao = 260,
    [Description("Erro na validação da Pass")]
    ErroValidacaoPass = 261,
    [Description("Erro na validação da Key")]
    ErroValidacaoKey = 262,
    [Description("Reservado para Impostos Retido na Fonte (porcentagens)")]
    ReservadoParaImpostosRetidoNaFontePorcentagens = 270,
    [Description("Erro de validação na porcentagem do campo porcentagemPIS")]
    ErroValidacaoPorcentagemCampoPorcentagemPIS = 271,
    [Description("Erro de validação na porcentagem do campo porcentagemCOFINS")]
    ErroValidacaoPorcentagemCampoPorcentagemCOFINS = 272,
    [Description("Erro de validação na porcentagem do campo porcentagemCSLL")]
    ErroValidacaoPorcentagemCampoPorcentagemCSLL = 273,
    [Description("Erro de validação na porcentagem do campo porcentagemIRRF")]
    ErroValidacaoPorcentagemCampoPorcentagemIRRF = 274,
    [Description("Erro de validação na porcentagem do campo porcentagemINSS")]
    ErroValidacaoPorcentagemCampoPorcentagemINSS = 275,
    [Description("Erro de validação na porcentagem do campo porcentagemOutros")]
    ErroValidacaoPorcentagemCampoPorcentagemOutros = 276,
    [Description("Reservado para Impostos Retido na Fonte (valores)")]
    ReservadoParaImpostosRetidoNaFonteValores = 280,
    [Description("Erro de validação na valor do campo valorPIS")]
    ErroValidacaoValorCampoValorPIS = 281,
    [Description("Erro de validação na valor do campo valorCOFINS")]
    ErroValidacaoValorCampoValorCOFINS = 282,
    [Description("Erro de validação na valor do campo valorCSLL")]
    ErroValidacaoValorCampoValorCSLL = 283,
    [Description("Erro de validação na valor do campo valorIRRF")]
    ErroValidacaoValorCampoValorIRRF = 284,
    [Description("Erro de validação na valor do campo valorINSS")]
    ErroValidacaoValorCampoValorINSS = 285,
    [Description("Erro de validação na valor do campo valorOutros")]
    ErroValidacaoValorCampoValorOutros = 286,
    [Description("Erro na validação do serviço")]
    ErroValidacaoServico = 290,
    [Description("Sua cidade não permite este serviço")]
    SuaCidadeNaoPermiteServico = 294,
    [Description("Solicitação de Integração Pendente")]
    SolicitacaoIntegracaoPendente = 295,
    [Description("Solicitação Indeferida")]
    SolicitacaoIndeferida = 296,
    [Description("Integração Bloqueada")]
    IntegracaoBloqueada = 297,
    [Description("Emissão de RPS não permitida em Homologação")]
    EmissaoRPSNaoPermitidaHomologacao = 298,
    [Description("Sua cidade não possui emissão de RPS")]
    SuaCidadeNaoPossuiEmissaoRPS = 299,
}