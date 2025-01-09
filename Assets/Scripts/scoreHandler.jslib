mergeInto(LibraryManager.library, {
    SendScoreToJS: function(score) {
        var jsScore = score;
        var event = new CustomEvent("gameProgress", {
            detail: { score: jsScore },
        });
        window.dispatchEvent(event);
    }
});