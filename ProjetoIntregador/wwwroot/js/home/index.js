$(document).ready(function () {    
    $.getJSON(`home/getmodelos`)
        .done(function (data) {
            var filiaisElement = document.getElementById("filiaisCount");
            var modelosElement = document.getElementById("modelosCount");
            var rmseElement = document.getElementById("rmse");
            filiaisElement.innerHTML = data.filiais;
            modelosElement.innerHTML = data.modelos;
            rmseElement.innerHTML = data.rmse;
        });
});