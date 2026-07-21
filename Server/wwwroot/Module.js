/* Module Script */
var GIBS = GIBS || {};

GIBS.Resource = {
};

window.downloadFile = function(fileName, contentType, base64Content) {
    const linkSource = `data:${contentType};base64,${base64Content}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
};