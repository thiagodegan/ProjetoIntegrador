$(document).ready(function () {
    const BASE_URL = "projecao";

    var dataSourceFiliais = new kendo.data.DataSource({
        transport: {
            read: function (options) {
                $.ajax({
                    type: "POST",
                    url: BASE_URL+"/getfiliais",
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        options.success(result);
                    },
                    error: function (result) {
                        options.error(result);
                    }
                });
            }
        }
    });

    $("#filiais").kendoMultiSelect({
        placeholder: "Selecione as Filiais...",
        dataTextField: "Nome",
        dataValueField: "Filial",
        dataSource: dataSourceFiliais
    });

    var dataSourceCategorias = new kendo.data.DataSource({
        transport: {
            read: function (options) {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + "/getcategorias",
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        options.success(result);
                    },
                    error: function (result) {
                        options.success(result);
                    }
                });
            }
        }
    });

    $("#categorias").kendoMultiSelect({
        placeholder: "Selecione as Categorias...",
        dataTextField: "Categoria",
        dataValueField: "Id",
        dataSource: dataSourceCategorias
    });

    function atualizaQuadros() {
        var data = dataSource.data();
        let realizado = 0;
        let previsto = 0;
        let erro = 0;
        var realizadoElement = document.getElementById("realizadoCard");
        var previstoElement = document.getElementById("previstoCard");
        var erroElement = document.getElementById("erroCard");

        if (data !== undefined && data !== null) {
            for(let i = 0; i < data.length; i++) {
                if (!data[i].ValorIsNull) {
                    realizado = realizado + data[i].ValorNull;
                    previsto = previsto + data[i].Previsao;
                }
            }
        }

        if (realizado > 0) {
            erro = (previsto-realizado)/realizado;
        }

        realizadoElement.innerHTML = Math.round(realizado);
        previstoElement.innerHTML = Math.round(previsto);
        erroElement.innerHTML = Math.round(erro * 10000)/100 + "%";
    }

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: function (options) {
                // make JSONP request to https://demos.telerik.com/kendo-ui/service/products
                var myJson = {
                    "DtIni": $("#dtini").val(),
                    "DtFim": $("#dtfim").val(),
                    "Filiais": $("#filiais").data("kendoMultiSelect").value(),
                    "Categorias": $("#categorias").data("kendoMultiSelect").value()
                };
                $.ajax({
                    type: "POST",
                    url: BASE_URL+"/getdados",
                    contentType: "application/json",
                    dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                    data: JSON.stringify(myJson),
                    success: function (result) {
                        // notify the data source that the request succeeded
                        options.success(result);
                        atualizaQuadros();
                    },
                    error: function (result) {
                        // notify the data source that the request failed
                        options.error(result);
                    }
                });
            },
            cache: false
        }
    });

    var dataSourceGrid = new kendo.data.DataSource({
        transport: {
            read: function (options) {
                // make JSONP request to https://demos.telerik.com/kendo-ui/service/products
                var myJson = {
                    "DtIni": $("#dtini").val(),
                    "DtFim": $("#dtfim").val(),
                    "Filiais": $("#filiais").data("kendoMultiSelect").value(),
                    "Categorias": $("#categorias").data("kendoMultiSelect").value()
                };
                $.ajax({
                    type: "POST",
                    url: BASE_URL+"/getdetalhes",
                    contentType: "application/json",
                    dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                    data: JSON.stringify(myJson),
                    success: function (result) {
                        // notify the data source that the request succeeded
                        options.success(result);
                    },
                    error: function (result) {
                        // notify the data source that the request failed
                        options.error(result);
                    }
                });
            },
            cache: false
        }
    });

    $("#chart").kendoChart({
        title: {
            text: "CMV Realizado x Previsto"
        },
        legend: {
            position: "bottom"
        },
        chartArea: {
            background: ""
        },
        seriesDefaults: {
            type: "line",
            style: "smooth"
        },
        series: [{
            name: "Realizado",
            field: "ValorNull",
            categoryField: "DiaFmt"
        },
        {
            name: "Previsto",
            field: "Previsao",
            categoryField: "DiaFmt"
            }],
        valueAxis: {
            labels: {
                format: "{0}"
            },
            line: {
                visible: false
            },
            axisCrossingValue: -10
        },
        categoryAxis: {
            majorGridLines: {
                visible: false
            },
            labels: {
                rotation: "auto"
            }
        },
        tooltip: {
            visible: true,
            format: "{0}",
            template: "#= series.name #: #= value #"
        }
    });

    $("#dgrid").kendoGrid({
        groupable: true,
        sortable: true,
        pageable: {
            pageSize: 5,
            buttonCount: 5,
            pageSizes: [5, 20, 50],
        },
        columns: [ {
            field: "Categoria",
            title: "Categoria",
            width: 500
        }, {
            field: "ValorNull",
            title: "Realizado"
        }, {
            field: "Previsao",
            title: "Previsto"
        },{
            field: "Erro",
            title: "Erro",
            format: "{0:p2}",
            sortable: {
                initialDirection: "desc"
              }
        }]
    });

    var chart = $("#chart").data("kendoChart");

    chart.setDataSource(dataSource);

    var dgrid = $("#dgrid").data("kendoGrid");
    dgrid.setDataSource(dataSourceGrid);


    $("#BtnFiltrar").click(function () {        
        dataSourceGrid.read();
        dataSource.read();
    });
});
