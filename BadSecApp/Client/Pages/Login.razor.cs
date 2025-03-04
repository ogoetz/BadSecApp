﻿using BadSecApp.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace BadSecApp.Client.Pages
{
    public class LoginBase : ComponentBase
    {
        protected Credentials ProposedCredentials { get; set; }

        protected string resultat;

        [Inject]
        protected HttpClient http { get; set; }

        protected override void OnInitialized()
        {
            ProposedCredentials = new Credentials();
            base.OnInitialized();
        }

        protected async void Creer()
        {
            HttpResponseMessage retour = await http.GetAsync("api/Authentication?login=" + ProposedCredentials.login + "&pwd=" + ProposedCredentials.pwd);
            // SECU (A07:2021-Identification and Authentication Failures) : vulnérabilité complexe à voir et a priori pas exploitable car URL en dur, mais si on se trompe sur l'URL ci-dessus, c'est WASM qui catche (y compris si préfixé par /api) et qui renvoie OK. Or, comme on se base juste sur le status code pour dire que c'est authentifié et pas sur un retour d'un token à repasser derrière, on est accepté dans tous les cas !!! 
            /*OGZ
            A07:2021-Identification et authentification de mauvaise qualité
            Si une exception est générée pour une raison ou une autre dans l'API d'authentification, le code de de retour est alors en succès.
            Il faut renforcer le processus.
            Par exemple, on renvoyant un token dans la request authorization pour le user connecté, et en le réutilisant systématiquement pour les appels suivants
            */
            resultat = retour.IsSuccessStatusCode ? "Vous êtes connecté en tant que " + ProposedCredentials.login : "Authentification incorrecte";
            this.StateHasChanged();

            Shared.NavMenu.SetMenusVisibility(
                MenuPersonnesVisible: retour.IsSuccessStatusCode, 
                MenuContratsVisible: retour.IsSuccessStatusCode && ProposedCredentials.login == "admin");
        }
    }
}
