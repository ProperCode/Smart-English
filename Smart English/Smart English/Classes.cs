namespace Smart_English
{
    enum Tryb
    {
        automatyczny,
        polautomatyczny,
        reczny
    }

    public class Sentence
    {
        public string sentence_en = "";
        public string sentence_pl = "";
        public bool remembered = false;

        public Sentence() { }

        public Sentence(string Sentence_en, string Sentence_pl, bool Remembered = false)
        {
            sentence_en = Sentence_en;
            sentence_pl = Sentence_pl;
            remembered = Remembered;
        }
    }

    public class Profile
    {
        public string name = "";
        public bool male = true;

        public Profile()
        { }

        public Profile(string Name, bool Male)
        {
            name = Name;
            male = Male;
        }
    }

    public class Sentences_base
    {
        public string name = "";
        public int sentences_nr = 0;
        public int current_word_nr = 1;        
        public int completions = 0;

        public Sentences_base()
        { }

        public Sentences_base(string Name, int Sentences_nr, int Current_word_nr, int Completions)
        {
            name = Name;
            sentences_nr = Sentences_nr;
            current_word_nr = Current_word_nr;
            completions = Completions;
        }
    }
}