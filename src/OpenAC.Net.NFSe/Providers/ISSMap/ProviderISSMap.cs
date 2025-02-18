
using System;
using System.Linq;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderISSMap : ProviderBase
{
    public ProviderISSMap(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        this.Name = "ISSMap";
        if (!this.Municipio.Parametros.TryGetValue(nameof(CodigoCidade), out var codCidade) || codCidade.IsEmpty())
            throw new OpenDFeException($"O provedor {this.Name} precisa que seja informado o parâmetro {nameof(CodigoCidade)}.");

        if (!this.Municipio.Parametros.TryGetValue(nameof(Key), out var key) || key.IsEmpty())
            throw new OpenDFeException($"O provedor {this.Name} precisa que seja informado o parâmetro {nameof(Key)}.");

        if (!this.Municipio.Parametros.TryGetValue(nameof(ChaveCriptografia), out var chaveCriptografia) || chaveCriptografia.IsEmpty())
            throw new OpenDFeException($"O provedor {this.Name} precisa que seja informado o parâmetro {nameof(ChaveCriptografia)}.");

        this.CodigoCidade = codCidade.ToInt32();
        this.Key = key.ToInt32();
        this.ChaveCriptografia = chaveCriptografia;
    }

    #region Properties

    public int CodigoCidade { get; }
    public int Key { get; }
    public string ChaveCriptografia { get; }

    #endregion

    public override NotaServico LoadXml(XDocument xml)
    {
        throw new System.NotImplementedException();
    }

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        xmlDoc.Add(WriteRps(nota));
        return xmlDoc.AsString(identado, showDeclaration);
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        xmlDoc.Add(WriteRps(nota));
        return xmlDoc.AsString(identado, showDeclaration);
    }

    private XElement? FormatTag(string tag, string valor) => this.AddTag(TipoCampo.Str, id: string.Empty, tag, 1, int.MaxValue, Ocorrencia.Obrigatoria, valor.EncryptIssMap(this.ChaveCriptografia));
    private XElement? FormatTag(string tag, decimal valor) => this.FormatTag(tag, valor.ToString("0.00;-0.00").Replace(",", "."));
    private XElement? FormatTag(string tag, int valor) => this.FormatTag(tag, valor.ToString());
    private XElement? FormatTag(string tag, DateTime data) => this.FormatTag(tag, data.ToString("dd/MM/yyyy HH:mm:ss"));

    private XElement WriteRps(NotaServico nota)
    {
        var rps = new XElement("rps");
        rps.Add(this.AddTag(TipoCampo.Int, id: string.Empty, tag: "key", 1, int.MaxValue, Ocorrencia.Obrigatoria, this.Key));
        rps.Add(this.FormatTag("pass", this.Configuracoes.WebServices.Senha));
        rps.Add(this.FormatTag("numero", nota.IdentificacaoRps.Numero));
        rps.Add(this.FormatTag("dataHoraEmissao", nota.IdentificacaoRps.DataEmissao));
        rps.Add(this.FormatTag("descServico", nota.Servico.Descricao));
        rps.Add(this.FormatTag("localExecucao", nota.Servico.Municipio));
        rps.Add(this.FormatTag("codigoServicoPrestado", nota.Servico.ItemListaServico));
        rps.Add(this.FormatTag("servicoPrestado", nota.Servico.Discriminacao));
        rps.Add(this.FormatTag("valorIss", nota.Servico.Valores.ValorIss + nota.Servico.Valores.ValorIssRetido));
        rps.Add(this.FormatTag("valorBaseCalculo", nota.Servico.Valores.BaseCalculo));
        rps.Add(this.FormatTag("valorDeducoes", nota.Servico.Valores.ValorDeducoes));
        rps.Add(this.FormatTag("valorNota", nota.Servico.Valores.ValorLiquidoNfse));
        rps.Add(this.FormatTag("aliquota", nota.Servico.Valores.Aliquota));
        rps.Add(this.FormatTag("cpfCnpjPrestador", nota.Prestador.CpfCnpj));
        rps.Add(this.FormatTag("nomeRazaoPrestador", nota.Prestador.RazaoSocial));
        rps.Add(this.FormatTag("enderecoPrestador", $"{nota.Prestador.Endereco.Logradouro} {nota.Prestador.Endereco.Numero}".Trim()));
        rps.Add(this.FormatTag("cidadePrestador", nota.Prestador.Endereco.Municipio));
        rps.Add(this.FormatTag("imPrestador", nota.Prestador.InscricaoMunicipal));
        rps.Add(this.FormatTag("ieRgPrestador", nota.Prestador.InscricaoEstadual));
        rps.Add(this.FormatTag("estadoPrestador", nota.Prestador.Endereco.Uf));
        rps.Add(this.FormatTag("cepPrestador", nota.Prestador.Endereco.Cep));
        rps.Add(this.FormatTag("cpfCnpjTomador", nota.Tomador.CpfCnpj));
        rps.Add(this.FormatTag("nomeRazaoTomador", nota.Tomador.RazaoSocial));
        rps.Add(this.FormatTag("enderecoTomador", $"{nota.Tomador.Endereco.Logradouro} {nota.Tomador.Endereco.Numero}".Trim()));
        rps.Add(this.FormatTag("cidadeTomador", nota.Tomador.Endereco.Municipio));
        rps.Add(this.FormatTag("cepTomador", nota.Tomador.Endereco.Cep));
        rps.Add(this.FormatTag("estadoTomador", nota.Tomador.Endereco.Uf));
        rps.Add(this.FormatTag("porcentagemPIS", nota.Servico.Valores.AliquotaPis));
        rps.Add(this.FormatTag("porcentagemCOFINS", nota.Servico.Valores.AliquotaCofins));
        rps.Add(this.FormatTag("porcentagemCSLL", nota.Servico.Valores.AliquotaCsll));
        rps.Add(this.FormatTag("porcentagemIRRF", nota.Servico.Valores.AliquotaIR));
        rps.Add(this.FormatTag("porcentagemINSS", nota.Servico.Valores.AliquotaInss));
        // rps.Add(this.FormatTag("porcentagemOutros", nota.Servico.Valores.));
        rps.Add(this.FormatTag("valorPIS", nota.Servico.Valores.ValorPis));
        rps.Add(this.FormatTag("valorCOFINS", nota.Servico.Valores.ValorCofins));
        rps.Add(this.FormatTag("valorCSLL", nota.Servico.Valores.ValorCsll));
        rps.Add(this.FormatTag("valorIRRF", nota.Servico.Valores.ValorIr));
        rps.Add(this.FormatTag("valorINSS", nota.Servico.Valores.ValorInss));
        // rps.Add(this.FormatTag("retencaoObrigatoriaTomador", "true"));
        rps.Add(this.FormatTag("retidoNaFonte", nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "S" : "N"));
        rps.Add(this.FormatTag("emailTomador", nota.Tomador.DadosContato.Email));
        rps.Add(this.FormatTag("emailPrestador", nota.Prestador.DadosContato.Email));
        return rps;
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override string GerarCabecalho() => string.Empty;

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        throw new System.NotImplementedException();
    }

    protected override string GetSchema(TipoUrl tipo) => string.Empty;

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new System.NotImplementedException();
    }
}