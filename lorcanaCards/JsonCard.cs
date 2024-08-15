using System;
using System.Collections.Generic;

namespace lorcanaCards
{
	public class JsonCard
	{
        public List<Ability> abilities { get; set; }
        public List<string> artists { get; set; }
        public string artistsText { get; set; }
        public string code { get; set; }
        public string color { get; set; }
        public int cost { get; set; }
        public string flavorText { get; set; }
        public List<string> foilTypes { get; set; }
        public string fullIdentifier { get; set; }
        public string fullName { get; set; }
        public string fullText { get; set; }
        public List<string> fullTextSections { get; set; }
        public int id { get; set; }
        public Images images { get; set; }
        public bool inkwell { get; set; }
        public int? lore { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public string rarity { get; set; }
        public string setCode { get; set; }
        public string simpleName { get; set; }
        public string story { get; set; }
        public int? strength { get; set; }
        public List<string> subtypes { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public int? willpower { get; set; }
        public List<string> keywordAbilities { get; set; }
        public List<int> promoIds { get; set; }
        public int? enchantedId { get; set; }
        public List<string> errata { get; set; }
        public List<string> clarifications { get; set; }
        public List<string> effects { get; set; }
        public string variant { get; internal set; }
    }

    public class Ability
    {
        public string effect { get; set; }
        public string fullText { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string keyword { get; set; }
        public string keywordValue { get; set; }
        public int? keywordValueNumber { get; set; }
        public string reminderText { get; set; }
        public List<string> costs { get; set; }
        public string costsText { get; set; }
    }

    public class Images
    {
        public string full { get; set; }
        public string thumbnail { get; set; }
        public string foilMask { get; set; }
    }
}

