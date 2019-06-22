using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MKLUODDD.Context {

    using Mapper;

    class DomainToOrmVisitor<T, TD> : ExpressionVisitor {
        IReadMapper<T, TD> Mapper { get; }
        IDomainMapper DomainMapper { get; }
        ParameterExpression Td { get; }

        public DomainToOrmVisitor(
            IReadMapper<T, TD> mapper, 
            IDomainMapper domainMapper,
            ParameterExpression td) {

            this.Mapper = mapper;
            this.DomainMapper = domainMapper;
            this.Td = td;
        }

        protected override Expression VisitMember(MemberExpression node) {

            if (node.Expression.Type == typeof(T))
                return Mapper.MapExpr(Td, node.Member);

            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {

            if ((node.Left is MemberExpression call) &&
                (node.Right is Expression arg))
                return Expression.MakeBinary(
                    node.NodeType,
                    Mapper.MapExpr(Td, call.Member),
                    Expression.MakeMemberAccess(
                        arg, arg.Type.GetMember("Value") [0]
                    )
                );
            else if ((node.Right is MemberExpression call2) &&
                (node.Left is Expression arg2))
                return Expression.MakeBinary(
                    node.NodeType,
                    Expression.MakeMemberAccess(
                        arg2, arg2.Type.GetMember("Value") [0]
                    ),
                    Mapper.MapExpr(Td, call2.Member)
                );
            else
                return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {

            if (!(node.Arguments.SingleOrDefault() is ConstantExpression arg))
                return base.VisitMethodCall(node);

            if (!(node.Object is MemberExpression call))
                return base.VisitMethodCall(node);

            return Expression.Call(
                Mapper.MapExpr(Td, call.Member),
                DomainMapper.MapMethod(arg.Value, node.Method), 
                new []{Expression.MakeMemberAccess(
                    arg, arg.Type.GetMember("Value") [0]
                )}
            );  
        }
    }
}