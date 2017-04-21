# Advanced Search

## Search GitHub issues in all Starcounter organizations

<iframe height="59" scrolling="no" src="/html/github_search_embed.html" frameborder="no" allowtransparency="true" allowfullscreen="true" style="width: 100%;"></iframe>

## Search for content on all Starcounter websites

<iframe height="59" scrolling="no" src="/html/web_search_embed.html" frameborder="no" allowtransparency="true" allowfullscreen="true" style="width: 100%;"></iframe>

## Useful searches

* [All open pull requests](https://github.com/search?o=desc&q=is%3Apr+is%3Aopen+user%3AStarcounter+user%3AStarcounterApps+user%3AJuicy+user%3AStarcounter-Jack+user%3APalindrom+user%3ASmorgasbord-Development&s=updated&type=Issues&utf8=%25E2%259C%2593)

<script>
document.addEventListener('DOMContentLoaded', function() {
    var frames = document.getElementsByTagName("IFrame");

    var errorCode = getErrorCode();
    if (errorCode === "") {
        return;
    }

    frames[0].addEventListener("load", function() {
        frames[0].contentDocument.getElementById("query").value = errorCode;
    })

    frames[1].addEventListener("load", function() {
        frames[1].contentDocument.getElementById("query").value = errorCode;
    })
});

function getErrorCode() {
    var url = window.location.href;
    var questionMarkIndex = url.indexOf("?");
    var parameterLength = "error=".length;
    if (url.slice(questionMarkIndex + 1, questionMarkIndex + parameterLength) === "error") {
        return url.slice(questionMarkIndex + parameterLength + 1);
    }
    return "";
}

</script>