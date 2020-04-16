function my_function(message) {
    console.log("from js file" + message);
}

function dotnetStaticInvokation() {
    DotNet.invokeMethodAsync("BlazorMovies.Client", "GetCurrentCount")
        .then(result => {
            console.log("count from js " + result);
        });
}

function dotnetInstanceInvokation(dotnetHelper) {
    dotnetHelper.invokeMethodAsync("IncrementCountInstance");
}