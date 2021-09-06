using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ReactWindowsAuth
{
	public class ClaimsTransformer : Microsoft.AspNetCore.Authentication.IClaimsTransformation
	{
		  
		public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{

			if (principal.Identity.AuthenticationType == "Negotiate")
			{
				var wi = (WindowsIdentity)principal.Identity;

				var claim = new Claim(wi.RoleClaimType, Constants.EmployeePolicy);
				wi.AddClaim(claim);
				
				 



			}
				return Task.FromResult(principal);
			 
				 
		}
	}
}
