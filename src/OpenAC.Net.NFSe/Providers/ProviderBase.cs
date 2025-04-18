// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 07-27-2014
//
// Last Modified By : Felipe (Transis Software)
// Last Modified On : 04-15-2023
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Class ProviderBase.
/// </summary>
public abstract class ProviderBase : IOpenLog, IDisposable
{
    #region Internal Types

    /// <summary>
    /// 
    /// </summary>
    protected enum TipoArquivo
    {
        /// <summary>
        /// 
        /// </summary>
        Webservice,
        /// <summary>
        /// 
        /// </summary>
        Rps,
        /// <summary>
        /// 
        /// </summary>
        NFSe
    }

    /// <summary>
    /// 
    /// </summary>
    protected enum Ocorrencia
    {
        /// <summary>
        /// 
        /// </summary>
        NaoObrigatoria,
        /// <summary>
        /// 
        /// </summary>
        Obrigatoria,
        /// <summary>
        /// 
        /// </summary>
        MaiorQueZero
    }

    #endregion Internal Types

    #region Fields

    private X509Certificate2? certificado;
    private bool disposed;

    #endregion Fields

    #region Constantes

    /// <summary>
    /// The er r_ ms g_ maior
    /// </summary>
    protected const string MsgMaior = "Tamanho maior que o máximo permitido";

    /// <summary>
    /// The er r_ ms g_ menor
    /// </summary>
    protected const string MsgMenor = "Tamanho menor que o mínimo permitido";

    /// <summary>
    /// The er r_ ms g_ vazio
    /// </summary>
    protected const string MsgVazio = "Nenhum valor informado";

    /// <summary>
    /// The er r_ ms g_ invalido
    /// </summary>
    protected const string MsgInvalido = "Conteúdo inválido";

    /// <summary>
    /// The er r_ ms g_ maxim o_ decimais
    /// </summary>
    protected const string MsgMaximoDecimais = "Numero máximo de casas decimais permitidas";

    /// <summary>
    /// The er r_ ms g_ maio r_ maximo
    /// </summary>
    protected const string MsgMaiorMaximo = "Número de ocorrências maior que o máximo permitido - Máximo ";

    /// <summary>
    /// The er r_ ms g_ fina l_ meno r_ inicial
    /// </summary>
    protected const string MsgFinalMenorInicial = "O numero final não pode ser menor que o inicial";

    /// <summary>
    /// The er r_ ms g_ arquiv o_ na o_ encontrado
    /// </summary>
    protected const string MsgArquivoNaoEncontrado = "Arquivo não encontrado";

    /// <summary>
    /// The er r_ ms g_ soment e_ um
    /// </summary>
    protected const string MsgSomenteUm = "Somente um campo deve ser preenchido";

    /// <summary>
    /// The er r_ ms g_ meno r_ minimo
    /// </summary>
    protected const string MsgMenorMinimo = "Número de ocorrências menor que o mínimo permitido - Mínimo ";

    /// <summary>
    /// The ds c_ CNPJ
    /// </summary>
    protected const string DscCnpj = "CNPJ(MF)";

    /// <summary>
    /// The ds c_ CPF
    /// </summary>
    protected const string DscCpf = "CPF";

    #endregion Constantes

    #region Constructor

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ProviderBase"/>.
    /// </summary>
    protected ProviderBase(ConfigNFSe config, OpenMunicipioNFSe municipio)
    {
        Name = "Base";
        ListaDeAlertas = new List<string>();
        FormatoAlerta = "TAG:%TAG% ID:%ID%/%TAG%(%DESCRICAO%) - %MSG%.";
        Configuracoes = config;
        Municipio = municipio;
        Versao = VersaoNFSe.ve100;
    }

    /// <inheritdoc />
    ~ProviderBase()
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }

    #endregion Constructor

    #region Propriedades

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; protected set; }
    
    /// <summary>
    /// 
    /// </summary>
    public VersaoNFSe Versao { get; protected set; }

    /// <summary>
    /// Gets the lista de alertas.
    /// </summary>
    /// <value>The lista de alertas.</value>
    public List<string> ListaDeAlertas { get; }

    /// <summary>
    /// Gets or sets the formato alerta.
    /// </summary>
    /// <value>The formato alerta.</value>
    public string FormatoAlerta { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [retirar acentos].
    /// </summary>
    /// <value><c>true</c> if [retirar acentos]; otherwise, <c>false</c>.</value>
    public bool RetirarAcentos { get; set; }
    
    /// <summary>
    /// Define/Obtém se deve usar vírgula nos números decimais.
    /// </summary>
    protected bool UsarVirgulaDecimais { get; set; }

    /// <summary>
    /// Obtém as configurações do componente
    /// </summary>
    public ConfigNFSe Configuracoes { get; }

    /// <summary>
    /// Obtém o Municipio atual.
    /// </summary>
    public OpenMunicipioNFSe Municipio { get; }

    /// <summary>
    /// Define/Obtém o tempo de espera pela resposta do provedor.
    /// </summary>
    public TimeSpan? TimeOut
    {
        get
        {
            TimeSpan? timeOut = null;
            if (Configuracoes.WebServices.AjustaAguardaConsultaRet)
                timeOut = TimeSpan.FromSeconds((int)Configuracoes.WebServices.AguardarConsultaRet);

            return timeOut;
        }
    }

    /// <summary>
    /// Obtém o certificado para ser usado na conexão ao provedor.
    /// </summary>
    public X509Certificate2 Certificado => certificado ??= Configuracoes.Certificados.ObterCertificado();

    #endregion Propriedades

    #region Methods

    #region Public

    #region XML

    /// <summary>
    /// Carrega o xml da NFSe/Rps.
    /// </summary>
    /// <param name="xml">Local do arquivo Xml</param>
    /// <param name="encoding">Enconde para utilizar na leitura do arquivo</param>
    /// <returns></returns>
    public NotaServico LoadXml(string xml, Encoding? encoding = null)
    {
        Guard.Against<ArgumentNullException>(xml.IsEmpty(), "Xml não pode ser vazio ou nulo");

        XDocument doc;
        if (File.Exists(xml))
        {
            if (encoding == null)
            {
                doc = XDocument.Load(xml);
            }
            else
            {
                using var sr = new StreamReader(xml, encoding);
                doc = XDocument.Load(sr);
            }
        }
        else
        {
            var byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(byteOrderMarkUtf8))
            {
                xml = xml.Replace(byteOrderMarkUtf8, "");
            }

            doc = XDocument.Parse(xml);
        }

        return LoadXml(doc);
    }

    /// <summary>
    /// Carrega o xml da NFSe/Rps.
    /// </summary>
    /// <param name="stream">Stream contendo os dados do arquivo xml.</param>
    /// <returns></returns>
    public NotaServico LoadXml(Stream stream)
    {
        Guard.Against<ArgumentNullException>(stream == null, "Stream não pode ser nulo !");

        var doc = XDocument.Load(stream);
        return LoadXml(doc);
    }

    /// <summary>
    /// Carrega o xml da NFSe/Rps.
    /// </summary>
    /// <param name="xml">Classe XDocument com um xml carregado.</param>
    /// <returns></returns>
    public abstract NotaServico LoadXml(XDocument xml);

    /// <summary>
    /// Retorna o xml da Rps em formato string.
    /// </summary>
    /// <param name="nota"></param>
    /// <param name="identado"></param>
    /// <param name="showDeclaration"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public abstract string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true);

    /// <summary>
    /// Retorna o xml da NFSe em formato string.
    /// </summary>
    /// <param name="nota"></param>
    /// <param name="identado"></param>
    /// <param name="showDeclaration"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public abstract string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true);

    #endregion XML

    #region Servicos

    /// <summary>
    /// Enviar coleção de Rps para o provedor de forma assíncrona.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="notas"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoEnviar Enviar(int lote, NotaServicoCollection notas)
    {
        var retornoWebservice = new RetornoEnviar()
        {
            Lote = lote,
            Sincrono = false
        };

        try
        {
            PrepararEnviar(retornoWebservice, notas);
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarEnviar(retornoWebservice);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"Enviar-{lote}-env.xml");

            //Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.Enviar))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.Enviar));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.Enviar))
            {
                var cabec = GerarCabecalho();
                retornoWebservice.XmlRetorno = cliente.Enviar(cabec, retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"lote-{lote}-ret.xml");
            TratarRetornoEnviar(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Enviar coleção de Rps para o provedor de forma síncrona.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="notas"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoEnviar EnviarSincrono(int lote, NotaServicoCollection notas)
    {
        var retornoWebservice = new RetornoEnviar()
        {
            Lote = lote,
            Sincrono = true
        };

        PrepararEnviarSincrono(retornoWebservice, notas);
        if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

        if (Configuracoes.Geral.RetirarAcentos)
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

        AssinarEnviarSincrono(retornoWebservice);
        GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"EnviarSincrono-{lote}-env.xml");

        //Remover a declaração do Xml se tiver
        retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

        // Verifica Schema
        if (PrecisaValidarSchema(TipoUrl.EnviarSincrono))
        {
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.EnviarSincrono));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;
        }

        // Recebe mensagem de retorno
        try
        {
            using var cliente = GetClient(TipoUrl.EnviarSincrono);
            //Separei o cabecalho em string pois dificultava o debug do envio, obrigando sempre a passar pelo GerarCabecalho
            var cabecalho = GerarCabecalho(); 
            retornoWebservice.XmlRetorno = cliente.EnviarSincrono(cabecalho, retornoWebservice.XmlEnvio);
            retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
            retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
            return retornoWebservice;
        }

        GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"EnviarSincrono-{lote}-ret.xml");
        TratarRetornoEnviarSincrono(retornoWebservice, notas);

        return retornoWebservice;
    }

    /// <summary>
    /// Consulta a situação do lote.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="protocolo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarSituacao ConsultarSituacao(int lote, string protocolo)
    {
        var retornoWebservice = new RetornoConsultarSituacao()
        {
            Lote = lote,
            Protocolo = protocolo
        };

        try
        {
            PrepararConsultarSituacao(retornoWebservice);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarConsultarSituacao(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            //Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.ConsultarSituacao))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarSituacao));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.ConsultarSituacao))
            {
                retornoWebservice.XmlRetorno = cliente.ConsultarSituacao(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");
            TratarRetornoConsultarSituacao(retornoWebservice);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Consultar o lote de rps.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="protocolo"></param>
    /// <param name="notas"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarLoteRps ConsultarLoteRps(int lote, string protocolo, NotaServicoCollection notas)
    {
        var retornoWebservice = new RetornoConsultarLoteRps()
        {
            Lote = lote,
            Protocolo = protocolo
        };

        try
        {
            PrepararConsultarLoteRps(retornoWebservice);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarConsultarLoteRps(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.ConsultarLoteRps))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarLoteRps));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.ConsultarLoteRps))
            {
                retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");
            TratarRetornoConsultarLoteRps(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Consulta o número da ultima nota fiscal de serviço emitida da serie informada.
    /// </summary>
    /// <param name="serie"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarSequencialRps ConsultarSequencialRps(string serie)
    {
        var retornoWebservice = new RetornoConsultarSequencialRps()
        {
            Serie = serie
        };

        try
        {
            PrepararConsultarSequencialRps(retornoWebservice);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarConsultarSequencialRps(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSequencialRps-{DateTime.Now:yyyyMMddssfff}-{serie}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.ConsultarSequencialRps))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarSequencialRps));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.ConsultarSequencialRps))
            {
                retornoWebservice.XmlRetorno = cliente.ConsultarSequencialRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSequencialRps-{DateTime.Now:yyyyMMddssfff}-{serie}-ret.xml");
            TratarRetornoConsultarSequencialRps(retornoWebservice);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Consulta uma NFSe usando o número do RPS.
    /// </summary>
    /// <param name="numero"></param>
    /// <param name="serie"></param>
    /// <param name="tipo"></param>
    /// <param name="notas"></param>
    /// <param name="anoCompetencia"></param>
    /// <param name="mesCompetencia"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSeRps ConsultaNFSeRps(int numero, string serie, TipoRps tipo, NotaServicoCollection notas, int anoCompetencia, int mesCompetencia)
    {
        var retornoWebservice = new RetornoConsultarNFSeRps()
        {
            NumeroRps = numero,
            Serie = serie,
            Tipo = tipo,
            AnoCompetencia = anoCompetencia,
            MesCompetencia = mesCompetencia
        };

        try
        {
            PrepararConsultarNFSeRps(retornoWebservice, notas);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarConsultarNFSeRps(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarNFSeRps-{numero}-{serie}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.ConsultarNFSeRps))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarNFSeRps));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.ConsultarNFSeRps))
            {
                retornoWebservice.XmlRetorno = cliente.ConsultarNFSeRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarNFSeRps-{numero}-{serie}-ret.xml");
            TratarRetornoConsultarNFSeRps(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro em ConsultaNFSeRps: " + ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Consulta uma ou mais NFSe de acordo com os filtros.
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="inicio"></param>
    /// <param name="fim"></param>
    /// <param name="numeroNfse"></param>
    /// <param name="serieNfse"></param>
    /// <param name="pagina"></param>
    /// <param name="cnpjTomador"></param>
    /// <param name="imTomador"></param>
    /// <param name="nomeInter"></param>
    /// <param name="cnpjInter"></param>
    /// <param name="imInter"></param>
    /// <returns></returns>
    public RetornoConsultarNFSe ConsultaNFSe(NotaServicoCollection notas, DateTime? inicio = null,
        DateTime? fim = null, int numeroNfse = 0, string serieNfse = "", int pagina = 0, string cnpjTomador = "",
        string imTomador = "", string nomeInter = "", string cnpjInter = "", string imInter = "")
    {
        var retornoWebservice = new RetornoConsultarNFSe()
        {
            Inicio = inicio,
            Fim = fim,
            NumeroNFse = numeroNfse,
            SerieNFse = serieNfse,
            Pagina = pagina,
            CPFCNPJTomador = cnpjTomador,
            IMTomador = imTomador,
            NomeIntermediario = nomeInter,
            CPFCNPJIntermediario = cnpjInter,
            IMIntermediario = imInter
        };

        try
        {
            PrepararConsultarNFSe(retornoWebservice);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarConsultarNFSe(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarNFSe-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.ConsultarNFSe))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarNFSe));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno
            using (var cliente = GetClient(TipoUrl.ConsultarNFSe))
            {
                retornoWebservice.XmlRetorno = cliente.ConsultarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarNFSe-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-ret.xml");
            TratarRetornoConsultarNFSe(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro em ConsultaNFSe: " + ex.Message });
            return retornoWebservice;
        }
    }

        /// <summary>
        /// Cancela uma NFSe.
        /// </summary>
        /// <param name="codigoCancelamento"></param>
        /// <param name="numeroNFSe"></param>
        /// <param name="motivo"></param>
        /// <param name="notas"></param>
        /// <returns></returns>
        public RetornoCancelar CancelarNFSe(string codigoCancelamento, string numeroNFSe, string serieNFSe, decimal valorNFSe, string motivo, string? codigoVerificacao, NotaServicoCollection notas)
        {
            var retornoWebservice = new RetornoCancelar()
            {
                CodigoCancelamento = codigoCancelamento,
                NumeroNFSe = numeroNFSe,
                SerieNFSe = serieNFSe,
                ValorNFSe = valorNFSe,
                Motivo = motivo,
                CodigoVerificacao = codigoVerificacao
            };

        try
        {
            PrepararCancelarNFSe(retornoWebservice);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarCancelarNFSe(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CancelarNFSe-{numeroNFSe}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.CancelarNFSe))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.CancelarNFSe));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.CancelarNFSe))
            {
                retornoWebservice.XmlRetorno = cliente.CancelarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CancelarNFSe-{numeroNFSe}-ret.xml");
            TratarRetornoCancelarNFSe(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro em CancelarNFSe: " + ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Cancela um lote de NFSe.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="notas"></param>
    /// <returns></returns>
    public RetornoCancelarNFSeLote CancelarNFSeLote(int lote, NotaServicoCollection notas)
    {
        var retornoWebservice = new RetornoCancelarNFSeLote()
        {
            Lote = lote
        };

        try
        {
            PrepararCancelarNFSeLote(retornoWebservice, notas);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarCancelarNFSeLote(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CancelarNFSeLote-{lote}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.CancelarNFSeLote))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.CancelarNFSeLote));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.CancelarNFSeLote))
            {
                retornoWebservice.XmlRetorno = cliente.CancelarNFSeLote(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CancelarNFSeLote-{lote}-ret.xml");
            TratarRetornoCancelarNFSeLote(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro em CancelarNFSeLote: " + ex.Message });
            return retornoWebservice;
        }
    }

    /// <summary>
    /// Substituie uma NFSe por outra.
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="codigoCancelamento"></param>
    /// <param name="numeroNFSe"></param>
    /// <param name="motivo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoSubstituirNFSe SubstituirNFSe(NotaServicoCollection notas, string codigoCancelamento, string numeroNFSe, string motivo)
    {
        var retornoWebservice = new RetornoSubstituirNFSe()
        {
            CodigoCancelamento = codigoCancelamento,
            NumeroNFSe = numeroNFSe,
            Motivo = motivo
        };

        try
        {
            PrepararSubstituirNFSe(retornoWebservice, notas);
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            AssinarSubstituirNFSe(retornoWebservice);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"SubstituirNFSe-{numeroNFSe}-env.xml");

            // Remover a declaração do Xml se tiver
            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoverDeclaracaoXml();

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.SubstituirNFSe))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.SubstituirNFSe));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno

            using (var cliente = GetClient(TipoUrl.SubstituirNFSe))
            {
                retornoWebservice.XmlRetorno = cliente.SubstituirNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"SubstituirNFSe-{numeroNFSe}-ret.xml");
            TratarRetornoSubstituirNFSe(retornoWebservice, notas);
            return retornoWebservice;
        }
        catch (Exception ex)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro em SubstituirNFSe: " + ex.Message });
            return retornoWebservice;
        }
    }

    #endregion Servicos

    #endregion Public

    #region Abstract

    /// <summary>
    /// Gera o xml de envio para o serviço de enviar.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    /// <param name="lote"></param>
    /// <returns></returns>
    protected abstract void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Gera o xml de envio para o serviço de enviar sincrono.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    /// <param name="lote"></param>
    /// <returns></returns>
    protected abstract void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Gera o xml de envio para o serviço de consultar situação.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="lote"></param>
    /// <param name="protocolo"></param>
    /// <returns></returns>
    protected abstract void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice);

    /// <summary>
    /// Gera o xml de envio para o serviço de consultar lote rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <returns></returns>
    protected abstract void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice);

    /// <summary>
    /// Gera o xml de envio para o serviço de consultar sequencial rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <returns></returns>
    protected abstract void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice);

    /// <summary>
    /// Gera o xml de envio para o serviço de consultar NFSe por RPS.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    /// <returns></returns>
    protected abstract void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Gera o xml de envio para o serviço consultar NFSe.
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="inicio"></param>
    /// <param name="fim"></param>
    /// <param name="numeroNfse"></param>
    /// <param name="pagina"></param>
    /// <param name="cnpjTomador"></param>
    /// <param name="imTomador"></param>
    /// <param name="nomeInter"></param>
    /// <param name="cnpjInter"></param>
    /// <param name="imInter"></param>
    /// <param name="serie"></param>
    /// <returns></returns>
    protected abstract void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice);

    /// <summary>
    /// Gera o xml de envio para o serviço cancelar NFSe.
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="codigoCancelamento"></param>
    /// <param name="numeroNFSe"></param>
    /// <param name="motivo"></param>
    /// <returns></returns>
    protected abstract void PrepararCancelarNFSe(RetornoCancelar retornoWebservice);

    /// <summary>
    /// Gera o xml de envio para o serviço cancelar NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    /// <param name="lote"></param>
    /// <returns></returns>
    protected abstract void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice,
        NotaServicoCollection notas);

    /// <summary>
    /// Gera o xml de envio para o serviço substituir NFSe.
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="codigoCancelamento"></param>
    /// <param name="numeroNFSe"></param>
    /// <param name="motivo"></param>
    /// <returns></returns>
    protected abstract void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Metodo para assinar o xml do serviço enviar.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarEnviar(RetornoEnviar retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço enviar sincrono.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarEnviarSincrono(RetornoEnviar retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço consultar situação.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço consultar lote rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço consultar sequencial rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço consultar NFSe por RPS.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço consultar NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço cancelar NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarCancelarNFSe(RetornoCancelar retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço cancelar NFSe lote.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice);

    /// <summary>
    /// Metodo para assinar o xml do serviço substituir NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice);

    /// <summary>
    /// Trata o retorno do enviar.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do enviar sincrono.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço de consultar situação.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice);

    /// <summary>
    /// Trata o retorno do serviço de consultar lote Rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço de consultar sequencial Rps.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    protected abstract void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice);

    /// <summary>
    /// Trata o retorno do serviço de consultar situação.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço consulta NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço cancelar NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço cancelar NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Trata o retorno do serviço substituir NFSe.
    /// </summary>
    /// <param name="retornoWebservice"></param>
    /// <param name="notas"></param>
    protected abstract void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas);

    /// <summary>
    /// Retorna o cliente de comunicação com o webservice.
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    protected abstract IServiceClient GetClient(TipoUrl tipo);

    /// <summary>
    /// Retorna o cabeçalho da mensagem.
    /// </summary>
    /// <returns></returns>
    protected abstract string GerarCabecalho();

    /// <summary>
    /// Retorna o schema xml para validação.
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    protected abstract string GetSchema(TipoUrl tipo);

    #endregion Abstract

    #region Protected

    /// <summary>
    /// Retorna a URL para o tipo de serviço.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>System.String.</returns>
    public string GetUrl(TipoUrl url)
    {
        var ret = Configuracoes.WebServices.Ambiente switch
        {
            DFeTipoAmbiente.Producao => Municipio.UrlProducao[url],
            DFeTipoAmbiente.Homologacao => Municipio.UrlHomologacao[url],
            _ => throw new ArgumentOutOfRangeException()
        };

        return ret.Replace("?wsdl", "");
    }

    /// <summary>
    /// Determinar ou não se deve validar o xml antes de enviar ao servidor.
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    protected virtual bool PrecisaValidarSchema(TipoUrl tipo) => true;

    /// <summary>
    /// Retornar o XML da assinatura ou nulo caso não tenha nada.
    /// </summary>
    /// <param name="signature">The signature.</param>
    /// <returns>XElement.</returns>
    protected XElement? WriteSignature(DFeSignature signature)
    {
        if (signature.SignatureValue.IsEmpty() || signature.SignedInfo.Reference.DigestValue.IsEmpty() ||
            signature.KeyInfo.X509Data.X509Certificate.IsEmpty())
            return null;

        var ms = new MemoryStream();
        var serializer = DFeSerializer<DFeSignature>.CreateSerializer<DFeSignature>();
        return !serializer.Serialize(signature, ms) ? null : XElement.Load(ms);
    }

    /// <summary>
    /// Carrega a assinatura do Xml.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>DFeSignature.</returns>
    protected DFeSignature LoadSignature(XElement? element)
    {
        if (element == null) return new DFeSignature();

        var serializer = DFeSerializer<DFeSignature>.CreateSerializer<DFeSignature>();
        return serializer.Deserialize(element.ToString());
    }

    /// <summary>
    /// Adicionars the tag CNPJCPF.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tagCPF">The i d1.</param>
    /// <param name="tagCNPJ">The i d2.</param>
    /// <param name="valor">The CNPJCPF.</param>
    /// <param name="ns"></param>
    /// <returns>XmlElement.</returns>
    protected XElement? AdicionarTagCNPJCPF(string id, string tagCPF, string tagCNPJ, string valor, XNamespace? ns = null)
    {
        valor = valor.Trim().OnlyNumbers();

        XElement? tag = null;
        switch (valor.Length)
        {
            case 11:
                tag = AddTag(TipoCampo.StrNumber, id, tagCPF, 11, 11, Ocorrencia.Obrigatoria, valor, string.Empty, ns);
                if (!valor.IsCPF())
                    WAlerta(tagCPF, "CPF", "CPF", MsgInvalido);
                break;

            case 14:
                tag = AddTag(TipoCampo.StrNumber, id, tagCNPJ, 14, 14, Ocorrencia.Obrigatoria, valor, string.Empty, ns);
                if (!valor.IsCNPJ())
                    WAlerta(tagCNPJ, "CNPJ", "CNPJ", MsgInvalido);
                break;
        }

        if (!valor.Length.IsIn(11, 14))
            WAlerta($"{tagCPF}-{tagCNPJ}", "CNPJ-CPF", "CNPJ/CPF", MsgVazio);

        return tag;
    }

    /// <summary>
    /// Adicionars the tag.
    /// </summary>
    /// <param name="tipo">The tipo.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <param name="ocorrencia">The ocorrencia.</param>
    /// <param name="valor">The valor.</param>
    /// <param name="descricao">The descricao.</param>
    /// <param name="ns"></param>
    /// <returns>XmlElement.</returns>
    protected XElement? AddTag(TipoCampo tipo, string id, string tag, XNamespace? ns, int min, int max, 
        Ocorrencia ocorrencia, object? valor, string descricao = "")
    {
        Guard.Against<ArgumentException>(ns == null, "Namespace não informado");

        return AddTag(tipo, id, tag, min, max, ocorrencia, valor, descricao, ns);
    }

    /// <summary>
    /// Adicionars the tag.
    /// </summary>
    /// <param name="tipo">The tipo.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <param name="ocorrencia">The ocorrencia.</param>
    /// <param name="valor">The valor.</param>
    /// <param name="descricao">The descricao.</param>
    /// <returns>XmlElement.</returns>
    protected XElement? AddTag(TipoCampo tipo, string id, string tag, int min, int max, Ocorrencia ocorrencia, object? valor, string descricao = "")
    {
        return AddTag(tipo, id, tag, min, max, ocorrencia, valor, descricao, null);
    }

    private XElement? AddTag(TipoCampo tipo, string id, string tag, int min, int max, Ocorrencia ocorrencia, object? valor, string descricao, XNamespace? ns)
    {
        try
        {
            var conteudoProcessado = string.Empty;
            var estaVazio = valor == null || valor.ToString().IsEmpty();

            if (!estaVazio)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (tipo)
                {
                    case TipoCampo.Str:
                        conteudoProcessado = valor!.ToString()!.Trim();
                        break;

                    case TipoCampo.Dat:
                    case TipoCampo.DatCFe:
                        if (DateTime.TryParse(valor!.ToString(), out var data))
                        {
                            conteudoProcessado = data.ToString(tipo == TipoCampo.DatCFe ? "yyyyMMdd" : "yyyy-MM-dd");
                        }
                        else
                        {
                            estaVazio = true;
                        }
                        break;

                    case TipoCampo.Hor:
                    case TipoCampo.HorCFe:
                        if (DateTime.TryParse(valor.ToString(), out var hora))
                        {
                            conteudoProcessado = hora.ToString(tipo == TipoCampo.HorCFe ? "HHmmss" : "HH:mm:ss");
                        }
                        else
                        {
                            estaVazio = true;
                        }
                        break;

                    case TipoCampo.DatHor:
                        if (DateTime.TryParse(valor.ToString(), out var dthora))
                            conteudoProcessado = dthora.ToString("s");
                        else
                            estaVazio = true;
                        break;

                    case TipoCampo.DatHorTz:
                        if (DateTime.TryParse(valor.ToString(), out var dthoratz))
                        {
                            conteudoProcessado = dthoratz.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
                        }
                        else
                        {
                            estaVazio = true;
                        }
                        break;

                    case TipoCampo.De2:
                    case TipoCampo.De3:
                    case TipoCampo.De4:
                    case TipoCampo.De6:
                    case TipoCampo.De10:
                        var cultureInfo = new CultureInfo(CultureInfo.InvariantCulture.Name);
                        if (UsarVirgulaDecimais)
                        {
                            cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
                        }
                        
                        if (decimal.TryParse(valor.ToString(), out var vDecimal))
                        {
                            if (ocorrencia == Ocorrencia.MaiorQueZero && vDecimal == 0)
                            {
                                estaVazio = true;
                            }
                            else
                            {
                                // ReSharper disable once SwitchStatementMissingSomeCases
                                conteudoProcessado = tipo switch
                                {
                                    TipoCampo.De2 => string.Format(cultureInfo, "{0:0.00}", vDecimal),
                                    TipoCampo.De3 => string.Format(cultureInfo, "{0:0.000}", vDecimal),
                                    TipoCampo.De4 => string.Format(cultureInfo, "{0:0.0000}", vDecimal),
                                    TipoCampo.De6 => string.Format(cultureInfo, "{0:0.000000}", vDecimal),
                                    _ => string.Format(cultureInfo, "{0:0.0000000000}", vDecimal)
                                };
                            }
                        }
                        else
                        {
                            estaVazio = true;
                        }

                        break;

                    case TipoCampo.Int:
                        if (int.TryParse(valor.ToString(), out var vInt))
                        {
                            if (ocorrencia == Ocorrencia.MaiorQueZero && vInt == 0)
                            {
                                estaVazio = true;
                            }
                            else
                            {
                                conteudoProcessado = valor.ToString();
                                if (conteudoProcessado.Length < min)
                                {
                                    conteudoProcessado = conteudoProcessado.ZeroFill(min);
                                }
                            }
                        }
                        else
                        {
                            estaVazio = true;
                        }
                        break;

                    case TipoCampo.StrNumberFill:
                        conteudoProcessado = valor.ToString();
                        if (conteudoProcessado.Length < min)
                        {
                            conteudoProcessado = conteudoProcessado.ZeroFill(min);
                        }
                        break;

                    case TipoCampo.StrNumber:
                        conteudoProcessado = valor.ToString().OnlyNumbers();
                        break;

                    default:
                        conteudoProcessado = valor.ToString();
                        break;
                }
            }

            var alerta = string.Empty;
            if (ocorrencia == Ocorrencia.Obrigatoria && estaVazio)
            {
                alerta = MsgVazio;
            }

            if (!conteudoProcessado.IsEmpty() && conteudoProcessado.Length < min && alerta.IsEmpty() && conteudoProcessado.Length > 1)
            {
                alerta = MsgMenor;
            }

            if (!conteudoProcessado.IsEmpty() && conteudoProcessado.Length > max)
            {
                alerta = MsgMaior;
            }

            if (!alerta.IsEmpty() && MsgVazio.Equals(alerta) && !estaVazio)
            {
                alerta += $" [{valor}]";
                WAlerta(id, tag, descricao, alerta);
            }

            XElement? xmlTag = null;
            if (ocorrencia == Ocorrencia.Obrigatoria && estaVazio)
                xmlTag = GetElement(tag, string.Empty, ns);

            return estaVazio ? xmlTag : GetElement(tag, conteudoProcessado, ns);
        }
        catch (Exception ex)
        {
            WAlerta(id, tag, descricao, ex.ToString());
            return GetElement(tag, string.Empty, ns);
        }
    }

    private static XElement GetElement(string name, string value, XNamespace? ns = null)
    {
        return ns != null ? new XElement(ns + name, value) : new XElement(name, value);
    }

    /// <summary>
    /// Ws the alerta.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="descricao">The descricao.</param>
    /// <param name="alerta">The alerta.</param>
    protected void WAlerta(string id, string tag, string descricao, string alerta)
    {
        // O Formato da mensagem de erro pode ser alterado pelo usuario alterando-se a property FFormatoAlerta: onde;
        // %TAG%       : Representa a TAG; ex: <nLacre>
        // %ID%        : Representa a ID da TAG; ex X34
        // %MSG%       : Representa a mensagem de alerta
        // %DESCRICAO% : Representa a Descrição da TAG

        var s = FormatoAlerta.Clone() as string;
        s = s.Replace("%ID%", id).Replace("%TAG%", $"<{tag}>")
            .Replace("%DESCRICAO%", descricao).Replace("%MSG%", alerta);

        ListaDeAlertas.Add(s);
        this.Log().Warn(s);
    }

    /// <summary>
    /// Valida o XML de acordo com o schema.
    /// </summary>
    /// <param name="retorno"></param>
    /// <param name="schema">O schema que será usado na verificação.</param>
    /// <returns>Se estiver tudo OK retorna null, caso contrário as mensagens de alertas e erros.</returns>
    protected virtual void ValidarSchema(RetornoWebservice retorno, string schema)
    {
        schema = Path.Combine(Configuracoes.Arquivos.PathSchemas, Name, Versao.GetDFeValue(), schema);
        if (!File.Exists(schema))
            retorno.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nao encontrou o arquivo schema do xml => " + schema });
            
        if (XmlSchemaValidation.ValidarXml(retorno.XmlEnvio, schema, out var errosSchema, out var alertasSchema)) return;

        foreach (var erro in errosSchema.Select(descricao => new EventoRetorno { Codigo = "0", Descricao = descricao }))
        {
            retorno.Alertas.Add(new EventoRetorno { Codigo = "0", Descricao = "Erro gerado ao validar o schema do xml => " + erro.Descricao });
            retorno.Erros.Add(erro);
        }

        foreach (var alerta in alertasSchema.Select(descricao => new EventoRetorno { Codigo = "0", Descricao = descricao }))
            retorno.Alertas.Add(alerta);
    }

    /// <summary>
    /// Grava o xml da Rps no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <param name="data"></param>
    protected void GravarRpsEmDisco(string conteudoArquivo, string nomeArquivo, DateTime data)
    {
        if (Configuracoes.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Rps, conteudoArquivo, nomeArquivo);
    }

    /// <summary>
    /// Grava o xml da NFSe no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <param name="data"></param>
    protected void GravarNFSeEmDisco(string conteudoArquivo, string nomeArquivo, DateTime data)
    {
        if (Configuracoes.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.NFSe, conteudoArquivo, nomeArquivo);
    }

    /// <summary>
    /// Grava o xml de comunicação com o webservice no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    protected void GravarArquivoEmDisco(string conteudoArquivo, string nomeArquivo)
    {
        if (Configuracoes.Geral.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Webservice, conteudoArquivo, nomeArquivo);
    }

    private void GravarArquivoEmDisco(TipoArquivo tipo, string conteudoArquivo, string nomeArquivo, DateTime? data = null)
    {
        nomeArquivo = tipo switch
        {
            TipoArquivo.Webservice => Path.Combine(
                Configuracoes.Arquivos.GetPathLote(data ?? DateTime.Today, Configuracoes.PrestadorPadrao.CpfCnpj),
                nomeArquivo),
            TipoArquivo.Rps => Path.Combine(
                Configuracoes.Arquivos.GetPathRps(data ?? DateTime.Today, Configuracoes.PrestadorPadrao.CpfCnpj),
                nomeArquivo),
            TipoArquivo.NFSe => Path.Combine(
                Configuracoes.Arquivos.GetPathNFSe(data ?? DateTime.Today, Configuracoes.PrestadorPadrao.CpfCnpj),
                nomeArquivo),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null)
        };

        File.WriteAllText(nomeArquivo, conteudoArquivo, Encoding.UTF8);
    }

    #endregion Protected

    #region Dispose

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implement IDisposable Interface.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            //
        }

#if NETFRAMEWORK
        certificado?.ForceUnload();
#else
            certificado?.Dispose();
#endif

        certificado = null;
        disposed = true;
    }

    #endregion Dispose

    #endregion Methods
}
