# Guides

Use the navigation on the left to learn how to use Starcounter's interfaces for databases, view-models and networking in your apps. We have made sure that you will feel at home by using APIs that are similar to those you already know.

## Essential Reading

Here's a recommended reading list for those that want to learn the most fundamental features of Starcounter:

{% import "../macros.html" as macros %}

{% set featured_subpages = [
    "guides/database/data-manipulation/README.md",
    "guides/database/creating-database-classes/README.md",
    "guides/transactions/using-transactions/README.md",
    "guides/typed-json/json-by-example/README.md",
    "guides/typed-json/code-behind/README.md",
    "guides/typed-json/json-data-bindings/README.md",
    "guides/working-with-starcounter/working-in-visual-studio/README.md",
    "guides/working-with-starcounter/administrator-web-ui/README.md",
    "guides/working-with-starcounter/starting-and-stopping-apps/README.md",
    "guides/web-apps/starcounter-mvvm/README.md",
    "guides/web-apps/client-side-stack/README.md",
    "guides/web-apps/html-views/README.md",
    "guides/network/handling-http-requests/README.md",
    "guides/network/creating-http-responses/README.md"
  ]
%}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles, featured_subpages) }}
