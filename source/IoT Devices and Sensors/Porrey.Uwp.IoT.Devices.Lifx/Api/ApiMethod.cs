using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public abstract class ApiMethod : IApi
	{
		private string _resource = string.Empty;
		private ILifxIdentity _identity = null;
		private HttpMethod _method = HttpMethod.Get;

		protected ApiMethod(ILifxIdentity identity, string resource, HttpMethod method)
		{
			this.Identity = identity;
			this.Resource = resource;
			this.Method = method;
		}

		protected virtual string Resource
		{
			get
			{
				return _resource;
			}
			set
			{
				_resource = value;
			}
		}

		protected ILifxIdentity Identity
		{
			get
			{
				return _identity;
			}

			set
			{
				_identity = value;
			}
		}

		protected HttpMethod Method
		{
			get
			{
				return _method;
			}
			set
			{
				this._method = value;
			}
		}
	}

	public abstract class ApiMethod<TResult> : ApiMethod
	{
		protected ApiMethod(ILifxIdentity identity, string resource)
			: base(identity, resource, HttpMethod.Get)
		{
		}

		protected async virtual Task<TResult> Execute(params string[] args)
		{
			TResult returnValue = default(TResult);

			HttpClient httpClient = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage();

			request.Headers.Authorization = this.Identity.Authentication;
			request.Method = HttpMethod.Get;
			request.RequestUri = new Uri(this.Identity.BaseUri, string.Format(this.Resource, args));

			HttpResponseMessage response = await httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				returnValue = await this.OnExecuted(response);
			}
			else
			{
				await this.OnFailed(response);
			}

			return returnValue;
		}

		protected virtual Task<TResult> OnExecuted(HttpResponseMessage response)
		{
			return Task<TResult>.FromResult(default(TResult));
		}

		protected virtual Task OnFailed(HttpResponseMessage response)
		{
			throw new HttpRequestException(response.ReasonPhrase);
		}
	}

	public abstract class ApiMethod<TResult, TPayload> : ApiMethod
	{
		protected ApiMethod(ILifxIdentity identity, string resource, HttpMethod method)
			: base(identity, resource, method)
		{

		}

		protected async virtual Task<TResult> Execute(TPayload body, params string[] args)
		{
			TResult returnValue = default(TResult);

			HttpClient httpClient = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage();

			string json = JsonConvert.SerializeObject(body);
			HttpContent bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

			request.Headers.Authorization = this.Identity.Authentication;
			request.Method = this.Method;
			request.RequestUri = new Uri(this.Identity.BaseUri, string.Format(this.Resource, args));
			request.Content = bodyContent;

			HttpResponseMessage response = await httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				returnValue = await this.OnExecuted(response);
			}
			else
			{
				await this.OnFailed(response);
			}

			return returnValue;
		}

		protected virtual Task<TResult> OnExecuted(HttpResponseMessage response)
		{
			return Task<TResult>.FromResult(default(TResult));
		}

		protected virtual Task OnFailed(HttpResponseMessage response)
		{
			throw new HttpRequestException(response.ReasonPhrase);
		}
	}
}
