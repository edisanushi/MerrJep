using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MerrJep.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExchangeRateConverterController : ControllerBase
	{
		[HttpGet("/api/euro-to-lek-rate")]
		public IActionResult EuroToLekRate()
		{
			var euroExchangeRate = Math.Round(ScrapeExchangeRate(), 5);
			return Ok(new { euroExchangeRate });
		}


		[HttpGet("/api/lek-to-euro-rate")]
		public IActionResult LekToEuroRate()
		{
			var euroExchangeRate = ScrapeExchangeRate();
			var lekExchangeRate = Math.Round((1 / euroExchangeRate), 5);
			return Ok(new { lekExchangeRate });
		}


		private decimal ScrapeExchangeRate()
		{
			try
			{
				string url = "https://www.bankofalbania.org/Tregjet/Kursi_zyrtar_i_kembimit/";
				HtmlWeb web = new HtmlWeb();
				HtmlDocument doc = web.Load(url);
				string xpath = $"(//td[contains(text(),'EUR')])[1]/following-sibling::td[1]";
				HtmlNode rateNode = doc.DocumentNode.SelectSingleNode(xpath);

				if (rateNode != null && decimal.TryParse(rateNode.InnerText.Trim(), out decimal rate))
				{
					return rate;
				}

				throw new Exception($"Exchange rate not found.");
			}
			catch (Exception ex)
			{
				throw new Exception($"Error scraping exchange rate: {ex.Message}");
			}
		}

	}
}
