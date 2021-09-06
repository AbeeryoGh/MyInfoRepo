using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;

namespace EngineCoreProject.DTOs.TabDto
{
    public class TabPostDto
    {
        public int? ParentId { get; set; }
        public Dictionary<string, string> NameShortCut { get; set; }
        public int TabOrder { get; set; }
        public string Link { get; set; }
        public byte[] Icon { get; set; }
        public string IconString { get; set; }
        public IFormFile IconImage { get; set; }
        public TabPostDto()
        {

        }
        public Tab GetEntity()
        {
            Tab tab = new Tab
            {
                ParentId = ParentId,
                TabOrder = TabOrder,
                Link = Link,
                Icon = Icon,
                IconString = IconString
            };

            return tab;
        }

    }
}
