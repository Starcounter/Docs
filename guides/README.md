# Guides

Use the navigation on the left to learn how to use Starcounter's interfaces for databases, view-models and networking in your apps. We have made sure that you will feel at home by using APIs that are similar to those you already know.

## Essential reading

To learn the fundamentals of Starcounter, read the articles listed below with a <i>&#9733;</i> next to it.

<style>
    .guide-articles {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
    }

    .guide-articles section {
        display: flex;
        flex-direction: column;
        width: 100%;
    }

    @media screen and (min-width: 768px) {
        .guide-articles section {
            width: 50%;
        }
    }

    @media screen and (min-width: 1024px) {
        .guide-articles section {
            width: 33.333%;
        }
    }
</style>

{% set recommended_reading = [
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
        "mapping and blending",
        "handling http requests",
        "transactions",
        "creating http responses",
        "using transactions"
    ]
%}

<h2>Articles in the guides:</h2>

<div class="guide-articles">
    {% for section in summary.parts[0].articles[3].articles %}
        <section>
        {% if recommended_reading.indexOf(section.title.toLowerCase()) != -1 %}
            <h3><a href="../{{ section.path }}">{{ section.title }}&#9733;</a></h3>
        {% else %}
            <h3><a href="../{{ section.path }}">{{ section.title }}</a></h3>
        {% endif %}
            {% for article in section.articles %}
                {% if recommended_reading.indexOf(article.title.toLowerCase()) != -1 %}
                    <a href="../../{{ article.path }}">{{ article.title }}&#9733;</a>
                {% else %}
                    <a href="../../{{ article.path }}">{{ article.title }}</a>
                {% endif %}
            {% endfor %}
        </section>
    {% endfor %}
</div>