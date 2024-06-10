namespace JiraApp.Models
{
    public class JiraResult<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public bool Success => Errors.Count == 0;

        // Constructor to initialize the Errors list
        public JiraResult()
        {
            Errors = new List<string>();
        }
    }
}
