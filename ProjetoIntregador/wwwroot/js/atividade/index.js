$(document).ready(function () {
    var refreshId = setInterval(function () {
        //GET YOUR GRID REFERENCE
        var grid = $("#dgrid").data("kendoGrid");
        grid.dataSource.read();


    }, 10000);
});

function executarClick(e) {
    e.preventDefault();
    // e.target is the DOM element representing the button
    var tr = $(e.target).closest("tr"); // get the current table row (tr)
    // get the data bound to the current table row
    var data = this.dataItem(tr);
    $.post(`atividade/StartAtividade?NomeAtividade=${data.NomeAtividade}`);
    var grid = $("#dgrid").data("kendoGrid");
    grid.dataSource.read();
}