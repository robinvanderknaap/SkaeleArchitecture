// This makes sure a call to console.log does not break on browser which don't support this call
// http://stackoverflow.com/a/1114200/426840
if (typeof console === "undefined") {
    console = { log: function () { } };
}