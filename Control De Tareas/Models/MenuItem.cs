namespace Control_De_Tareas.Models
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }    
        public List<string> Roles { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}

