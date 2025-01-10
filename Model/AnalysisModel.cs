namespace twitter_service.Model;

public class AnalysisModel
{
    public int Id { get; set; }
    //public int ChararactersCount { get; set; }
    public Sentiment Sentiment { get; set; }
    public List<Keywords> KeywordList { get; set; }
    public List<Entity> EntitiesList { get; set; }
    public Summary AnaylsisSummary { get; set; }
}

public class Keywords
{
    public int Id { get; set; }
    public string Keyword { get; set; }
    public double Relevance { get; set; }
    public Emotion EmotionList { get; set; }
}

public class Emotion
{
    int id { get; set; }
    public double Sadness { get; set; }
    public double Joy { get; set; }
    public double Fear { get; set; }
    public double Disgust { get; set; }
    public double Anger { get; set; }
}


// public class Entities
// {
//     public int Id { get; set; }
//     List<Entities> EntitiesList { get; set; }
// }
public class Entity
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Text { get; set; }
    public Sentiment? Sentiment { get; set; }
    public double Relavance{ get; set; }
    public int Count { get; set; }
    public double Confidence { get; set; }
}

public class Sentiment
{
    public int Id { get; set; }
    public double Score { get; set; }
    public string Mixed { get; set; }
    public  string Label { get; set; }
}

public class Summary
{
    public int Id { get; set; }
    public string? TextSummary { get; set; }
}



