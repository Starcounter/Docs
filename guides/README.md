# Guides

Use the navigation on the left to learn how to use Starcounter's interfaces for databases, view-models and networking in your apps. We have made sure that you will feel at home by using APIs that are similar to those you already know.

## Essential reading

To learn the fundamentals of Starcounter, read the articles listed below with a ★ next to it.

{% set recommended\_reading = \[  
        "database",  
        "typed json",  
        "working in visual studio",  
        "starcounter mvvm",  
        "creating database classes",  
        "json-by-example",  
        "administrator web ui",  
        "client-side stack",  
        "data manipulation",  
        "code-behind",  
        "starting and stopping apps",  
        "html views",  
        "sql",  
        "json data bindings",  
        "blending",  
        "handling http requests",  
        "transactions",  
        "creating http responses",  
        "using transactions"  
    \]  
%}

## Articles in the guides:

 {% for section in summary.parts\[0\].articles\[4\].articles %} {% if recommended\_reading.indexOf\(section.title.toLowerCase\(\)\) != -1 %}

### [{{ section.title }}★](https://github.com/Starcounter/Docs/tree/a457da51e4f68712de07c6ce90f9b1987ce2f3db/%7B%7B%20section.path%20%7D%7D)

 {% else %}

### [{{ section.title }}](https://github.com/Starcounter/Docs/tree/a457da51e4f68712de07c6ce90f9b1987ce2f3db/%7B%7B%20section.path%20%7D%7D)

 {% endif %} {% for article in section.articles %} {% if recommended\_reading.indexOf\(article.title.toLowerCase\(\)\) != -1 %} [{{ article.title }}★](https://github.com/Starcounter/Docs/tree/a457da51e4f68712de07c6ce90f9b1987ce2f3db/%7B%7B%20article.path%20%7C%20urlize%289,%20true%29%20%7C%20safe%7D%7D) {% else %} [{{ article.title }}](https://github.com/Starcounter/Docs/tree/a457da51e4f68712de07c6ce90f9b1987ce2f3db/%7B%7B%20article.path%20%7C%20urlize%289,%20true%29%20%7C%20safe%7D%7D) {% endif %} {% endfor %} {% endfor %}

