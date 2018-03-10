<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Positions.aspx.cs" Inherits="Gold.SystemSetting.Positions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <h2>
            <span>岗位列表</span></h2>
        <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
            DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Position"
            EnableInsert="True" EnableUpdate="True">
        </asp:EntityDataSource>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="PositionId"
            DataSourceID="EntityDataSource1" ShowFooter="true" ShowHeaderWhenEmpty="true" CssClass="linetable"
            OnRowCreated="GridView1_RowCreated">
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
            <Columns>
                <asp:BoundField DataField="PositionId" HeaderText="岗位ID" ControlStyle-Height="30px"
                    ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" ReadOnly="True" SortExpression="PositionId" />
                <asp:TemplateField HeaderText="岗位名称" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Center"
                    ItemStyle-VerticalAlign="Bottom" ControlStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Bind("PositionName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("PositionName") %>' MaxLength="32" />
                        <asp:RequiredFieldValidator ID="CodeRequired" runat="server" ControlToValidate="txtName"
                            CssClass="failureNotification" ErrorMessage="必须填写岗位名称" ToolTip="岗位名称不能为空！" ValidationGroup="UpdateValidationGroup">必须输入</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("PositionName") %>' MaxLength="32" />
                        <asp:RequiredFieldValidator ID="CodeRequired" runat="server" ControlToValidate="txtName"
                            CssClass="failureNotification" ErrorMessage="必须填写岗位名称" ToolTip="岗位名称不能为空！" ValidationGroup="InsertValidationGroup">必须输入</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="岗位描述" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Center"
                    ItemStyle-VerticalAlign="Middle" ControlStyle-Height="30px">
                    <ItemTemplate>
                        <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'
                            MaxLength="32" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'
                            MaxLength="32" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center"
                    ItemStyle-VerticalAlign="Middle" ControlStyle-Height="30px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit"
                            Text="编 辑" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update"
                            ValidationGroup="UpdateValidationGroup" Text="更 新" />
                        <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="取 消" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="lbtnInsert" runat="server" OnClick="lbtnInsert_Click" Text="插 入"
                            ValidationGroup="InsertValidationGroup" />
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <tr>
                    <td align="center" style="width: 100px;">
                    </td>
                    <td align="center" valign="bottom" style="width: 300px;">
                        <asp:TextBox ID="txtName2" MaxLength="32" runat="server" Text='<%# Bind("PositionName") %>' />
                        <asp:RequiredFieldValidator ID="CodeRequired" runat="server" ControlToValidate="txtName2"
                            CssClass="failureNotification" ErrorMessage="必须填写岗位名称" ToolTip="岗位名称不能为空！" ValidationGroup="InsertValidationGroup2">必须输入</asp:RequiredFieldValidator>
                    </td>
                    <td align="center" valign="middle" style="width: 300px;">
                        <asp:TextBox ID="txtDescption2" MaxLength="32" runat="server" Text='<%# Bind("Description") %>' />
                    </td>
                    <td align="center" valign="middle" style="width: 120px;">
                        <asp:LinkButton ID="lbtnSave" runat="server" OnClick="lbtnSave_Click" Text="插 入"
                            ValidationGroup="InsertValidationGroup2" />
                    </td>
                </tr>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>
