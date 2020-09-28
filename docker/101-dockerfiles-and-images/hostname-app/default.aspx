<%@ Import Namespace="System" %>
<%@ Page Language="c#"%>

<script runat="server">
    public string GetMachineName()
    {
        return Environment.MachineName;
    }
</script>

<html>
    <head>
        <style>
        body {
        	background: #fdf6e3;
            background-color: #fdf6e3;
    		margin-bottom: 0px!important;
        }

        div{
            font-family: 'Geomanist', sans-serif;
  			font-weight: normal; 			
            color: #657b83;
            width: 85%;
            margin: 0 auto;
            position: relative;
            margin-top: 180px;
            transform: translateY(-50%);
        }

        .footer {
            position: fixed;
            bottom: 0;
            font-size: 14pt;
            text-align: center;
        }

        h1{
            font-size: 50pt
        }
        h2{
            font-size: 28pt
        }
        </style>
    </head>

    <body>
        <div>
            <h1>Hello from <% =GetMachineName() %>!</h1>
        </div>
        <div class="footer">
            A very simple demo app from the workshop <a href="https://dak4.net">Docker and Kubernetes for .NET Developers</a>
        </div>
    </body>

</html>