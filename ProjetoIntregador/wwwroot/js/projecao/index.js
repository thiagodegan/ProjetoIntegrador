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

    var chart = $("#chart").data("kendoChart");

    chart.setDataSource(dataSource);


    $("#BtnFiltrar").click(function () {
        dataSource.read();
    });
});
