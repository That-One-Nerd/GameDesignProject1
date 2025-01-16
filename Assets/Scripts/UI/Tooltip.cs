using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : Singleton<Tooltip>
{
    private List<Request> requests;

    private Image background;
    private TextMeshProUGUI title, content, footer;

    protected override void Awake()
    {
        base.Awake();
        requests = new List<Request>();

        background = GetComponent<Image>();
        title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        content = transform.Find("Content").GetComponent<TextMeshProUGUI>();
        footer = transform.Find("Footer").GetComponent<TextMeshProUGUI>();
    }

    private bool setToMousePos = true;
    private void Update()
    {
        Color desiredColor;
        Vector2 desiredPos = transform.position;
        if (requests.Count > 0)
        {
            desiredColor = Color.white;
            Request priority = requests[0];
            title.text = priority.Title;
            content.text = priority.Content;
            footer.text = priority.Footer;

            if (priority.Position.HasValue)
            {
                desiredPos = priority.Position.Value;
                setToMousePos = false;
            }
            else setToMousePos = true;
        }
        else desiredColor = new Color(1, 1, 1, 0);

        if (setToMousePos) desiredPos = Input.mousePosition;

        background.color = Color.Lerp(background.color, desiredColor, Time.deltaTime * 10);
        title.color = Color.Lerp(title.color, desiredColor, Time.deltaTime * 10);
        content.color = Color.Lerp(content.color, desiredColor, Time.deltaTime * 10);
        footer.color = Color.Lerp(footer.color, desiredColor, Time.deltaTime * 10);

        transform.position = Vector2.Lerp(transform.position, desiredPos, Time.deltaTime * 10);
    }

    public void SetRequest(Request request)
    {
        requests.Add(request);
        requests.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }
    public void RemoveRequest(object reference) => requests.RemoveAll(x => x.Reference == reference);

    public class Request
    {
        public object Reference;
        public int Priority;
        public string Title;
        public string Content;
        public string Footer;
        public Vector2? Position;
    }
}
