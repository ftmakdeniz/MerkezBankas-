using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Xml;

namespace MerkezBankasindan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GunlukKur : ControllerBase
    {
        [HttpPut]
        public ResponseData Run(RequestData request)
        {
            ResponseData result = new ResponseData();
            try
            {
                string tcmblink = string.Format("http://www.tcmb.gov.tr/kurlar/{0}.xml",
                    (request.IsBugun) ? "Today" : string.Format("{2}{1}/{0}{1}{2}", request.Gun.ToString().PadLeft(2, '0'),
                    request.Ay.ToString().PadLeft(2, '0'),
                    request.Yil
                    )
                    );
                result.Data = new List<ResponseDataKur>();
                XmlDocument doc = new XmlDocument();
                doc.Load(tcmblink);
                if (doc.SelectNodes("Tarih_Data").Count < 1) ;
                {
                    result.Hata = "Kur Bilgisi Bulunamadı";
                    return result;
                }
                foreach (XmlNode node in doc.SelectNodes("Tarih_Date")[0].ChildNodes)
                {
                    ResponseDataKur kur = new ResponseDataKur();
                    kur.Kodu = node.Attributes["Kod"].Value;
                    kur.Adi = node["Isim"].InnerText;
                    kur.Birimi = int.Parse(node["Unit"].InnerText);
                    kur.AlisKuru = Convert.ToDecimal("0" + node["ForexBuying"].InnerText.Replace(".", ","));
                    kur.SatisKuru = Convert.ToDecimal("0" + node["ForexSelling"].InnerText.Replace(".", ","));
                    kur.EfektifAlisKuru = Convert.ToDecimal("0" + node["BaknoteBuying"].InnerText.Replace(".", ","));
                    kur.efektifSatiskuru = Convert.ToDecimal("0" + node["BaknoteSelling"].InnerText.Replace(".", ","));

                    result.Data.Add(kur);
                }
            }   
            catch ( Exception ex )
            {
                result.Hata = ex.Message;

            }
            
            
            
            
            
            
            return result;
            
        }
    }
}
