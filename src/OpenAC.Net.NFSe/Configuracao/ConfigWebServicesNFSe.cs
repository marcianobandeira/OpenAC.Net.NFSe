// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ConfigWebServicesNFSe.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Net;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Configuracao;

public sealed class ConfigWebServicesNFSe : DFeWebserviceConfigBase
{
    #region Fields

    private int codigoMunicipio;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigWebServicesNFSe"/> class.
    /// </summary>
    internal ConfigWebServicesNFSe()
    {
        Usuario = string.Empty;
        Senha = string.Empty;
        FraseSecreta = string.Empty;
        ChaveAcesso = string.Empty;
        Protocolos = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Uf do webservice em uso
    /// </summary>
    /// <value>The uf.</value>
    public string Municipio { get; private set; }

    public NFSeProvider Provider { get; private set; } = NFSeProvider.Nenhum;

    public string Usuario { get; set; }

    public string Senha { get; set; }

    public string FraseSecreta { get; set; }

        public string ChaveAcesso { get; set; }
        public string ChavePrivada { get; set; }
        public string Proxy { get; set; }

    /// <summary>
    /// Codigo do municipio do Webservices em uso
    /// </summary>
    /// <value>The uf codigo.</value>
    public int CodigoMunicipio
    {
        get => codigoMunicipio;
        set
        {
            if (codigoMunicipio == value) return;

            var municipio = ProviderManager.Municipios.SingleOrDefault(x => x.Codigo == value);
            Guard.Against<ArgumentException>(municipio == null, "Município não cadastrado.");

            codigoMunicipio = value;
            Municipio = municipio?.Nome ?? string.Empty;
            Provider = municipio?.Provedor ?? NFSeProvider.Nenhum;
        }
    }

    #endregion Properties
}