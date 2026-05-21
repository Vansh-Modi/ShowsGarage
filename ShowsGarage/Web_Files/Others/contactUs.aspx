<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="contactUs.aspx.cs" Inherits="ShowsGarage.Web_Files.Others.contactUs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Contact Us </title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /* Base Container Styles */
        a{
            color : #aec6c9;
            text-decoration: none;
        }
        .garage-content {
            background: #121212;
            color: #aec6c9;
            padding: 40px;
            border-radius: 15px;
            border: 1px solid #333;
            max-width: 900px;
            margin: 20px auto;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .garage-header {
            color: #FAEAB1;
            border-bottom: 2px solid #FAEAB1;
            padding-bottom: 10px;
            margin-bottom: 25px;
            text-transform: uppercase;
            letter-spacing: 2px;
            font-size: 1.8rem;
        }

        /* Desktop Grid (Default) */
        .contact-grid {
            display: grid;
            grid-template-columns: 1.5fr 1fr;
            gap: 30px;
        }

        .input-group {
            margin-bottom: 20px;
        }

            .input-group label {
                display: block;
                margin-bottom: 8px;
                color: #fff;
                font-weight: bold;
            }

        .garage-input {
            width: 100%;
            padding: 14px;
            background: #1e1e1e;
            border: 1px solid #444;
            color: #fff;
            border-radius: 8px;
            box-sizing: border-box; /* Ensures padding doesn't break width */
            transition: 0.3s ease;
        }

            .garage-input:focus {
                border-color: #FAEAB1;
                outline: none;
                background: #252525;
            }

        .garage-btn {
            background: #FAEAB1;
            color: #121212;
            border: none;
            padding: 16px;
            font-weight: bold;
            cursor: pointer;
            border-radius: 8px;
            width: 100%;
            transition: 0.3s;
            font-size: 1rem;
            letter-spacing: 1px;
        }

            .garage-btn:hover {
                background: #e0d19d;
                transform: translateY(-2px);
            }

        .info-card {
            background: #1e1e1e;
            padding: 25px;
            border-radius: 12px;
            border-top: 4px solid #FAEAB1; /* Changed to top for mobile flow */
            height: fit-content;
        }

        /* --- MOBILE RESPONSIVENESS --- */
        @media (max-width: 768px) {
            .garage-content {
                margin: 10px;
                padding: 20px;
            }

            .contact-grid {
                grid-template-columns: 1fr; /* Stacks the columns */
                gap: 40px;
            }

            .garage-header {
                font-size: 1.4rem;
                text-align: center;
            }

            .info-card {
                order: 2; /* Puts the contact info below the form on mobile */
                text-align: center;
            }

            .garage-btn {
                padding: 18px; /* Larger hit area for thumbs */
            }
        }
    </style>

    <div class="garage-content">
        <h2 class="garage-header">Get In Touch</h2>
        <div class="contact-grid">
            <!-- Form Section -->
            <div class="form-section">
                <div class="input-group">
                    <label>Full Name<asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtContactName" Display="Dynamic" ErrorMessage="Enter Name" ForeColor="Red"></asp:RequiredFieldValidator>
                    </label>
                    &nbsp;<asp:TextBox ID="txtContactName" runat="server" CssClass="garage-input" placeholder="Your Name" CausesValidation="True"></asp:TextBox>
                </div>
                <div class="input-group">
                    <label>Email Address<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtContactEmail" Display="Dynamic" ErrorMessage="Enter Email" ForeColor="Red"></asp:RequiredFieldValidator>
                    </label>
                    &nbsp;<asp:TextBox ID="txtContactEmail" runat="server" CssClass="garage-input" placeholder="name@example.com" CausesValidation="True" TextMode="Email"></asp:TextBox>
                </div>
                <div class="input-group">
                    <label>Message<asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage" Display="Dynamic" ErrorMessage="Enter Message" ForeColor="Red"></asp:RequiredFieldValidator>
                    </label>
                    &nbsp;<asp:TextBox ID="txtMessage" runat="server" CssClass="garage-input" TextMode="MultiLine" Rows="5" placeholder="Tell us about your car..." CausesValidation="True"></asp:TextBox>
                </div>
                <asp:Button ID="btnSendMessage" runat="server" Text="SEND MESSAGE" CssClass="garage-btn" OnClick="btnSendMessage_Click" />
            </div>

            <!-- Info Section -->
            <div class="info-card">
                <h4 style="color: #fff; margin-top: 0; font-size: 1.2rem;">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </h4>
                <h4 style="color: #fff; margin-top: 0; font-size: 1.2rem;">Garage Hub</h4>
                <p>📍<a href="https://www.google.com/maps/dir//Avadh+Carolina,+Silent+Zone+Rd,+Gaviyer,+Surat,+Dumas,+Gujarat+394550/@21.2049469,72.7688223,15z/data=!4m8!4m7!1m0!1m5!1m1!1s0x3be052b0b5a735b5:0x1067fd7cd32c301c!2m2!1d72.7216156!2d21.1250289?entry=ttu&g_ep=EgoyMDI2MDUxMC4wIKXMDSoASAFQAw%3D%3D">Surat, Gujarat, India</a></p>
                <p>📧 <a href="vanshmodi268@gmail.com">vanshmodi268@gmail.com</a></p>
                <p>📞 +91 99986 77425</p>
                <%--<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>--%>
                <hr style="border: 0; border-top: 1px solid #444; margin: 20px 0;">
                <p style="font-size: 14px; color: #FAEAB1; font-style: italic;">
                    "Precision performance for the modern driver."
                </p>
            </div>
        </div>
    </div>
</asp:Content>
